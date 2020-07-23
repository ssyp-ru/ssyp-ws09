// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TheGrapho.Parser.Syntax;

namespace TheGrapho.Parser
{
    public sealed class Parser
    {
        public Parser([DisallowNull] IReadOnlyList<SyntaxToken> tokens)
        {
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        [NotNull] private readonly IReadOnlyList<SyntaxToken> _tokens;

        private int _currentTokenOffset;
        [NotNull] private List<(string, SyntaxToken?)> _errors = new List<(string, SyntaxToken?)>();

        [MaybeNull]
        private SyntaxToken CurrentToken
        {
            [DebuggerStepThrough] get => _tokens.ElementAtOrDefault(_currentTokenOffset);
        }

        [return: NotNull]
        public DotGraphSyntax Parse()
        {
            var graph = ParseGraph() ?? throw new ParserException(_errors);
            NextToken();
            if (CurrentToken == null) return graph;
            var unparsed = new StringBuilder();
            CurrentToken.Write(unparsed);
            Console.WriteLine(CurrentToken);
            Error($"Unparsed remainder - \"{unparsed}\"");
            throw new ParserException(_errors);
        }

        [return: MaybeNull]
        private DotGraphSyntax ParseGraph()
        {
            KeywordSyntax? strict = null;
            KeywordSyntax? graph;
            var strictDigraphOrGraph = CurrentToken;

            if (strictDigraphOrGraph == null)
            {
                Error($"Expecting strict, graph or digraph at {CurrentToken?.Start}.");
                return null;
            }

            if (strictDigraphOrGraph.Kind == SyntaxKind.StrictToken)
            {
                if (!(strictDigraphOrGraph is KeywordSyntax)) return null;
                strict = (KeywordSyntax) strictDigraphOrGraph;
                NextToken();
                var graphToken = CurrentToken;

                if (!(graphToken is KeywordSyntax ks) ||
                    !new[] {SyntaxKind.GraphToken, SyntaxKind.DigraphToken}.Contains(graphToken.Kind))
                {
                    Error($"Expecting graph after strict at {CurrentToken?.Start}.");
                    return null;
                }

                graph = ks;
            }
            else
                graph =
                    strictDigraphOrGraph.RequireKind(SyntaxKind.GraphToken, SyntaxKind.DigraphToken) as KeywordSyntax;

            if (graph == null)
            {
                Error($"Expecting graph declaration at {CurrentToken?.Start}.");
                return null;
            }

            NextToken();
            var id = Backtracking(it => it.ParseId());
            var lbr = PeekToken<PunctuationSyntax>(SyntaxKind.LeftCurlyBracketToken);

            if (lbr == null)
            {
                Error($"Expecting {{ after graph declaration at {CurrentToken?.Start}.");
                return null;
            }

            NextToken();
            var statementList = Backtracking(it => it.ParseStatementList(), false);

            if (statementList == null)
            {
                Error($"Expecting statement list after strict at {CurrentToken?.Start}.");
                return null;
            }

            var rbr = PeekToken<PunctuationSyntax>(SyntaxKind.RightCurlyBracketToken);
            if (rbr != null) return new DotGraphSyntax(strict, graph, id, lbr, statementList, rbr);
            Error($"Expecting }} after statement list at {CurrentToken?.Start}.");
            return null;
        }

        [return: MaybeNull]
        private DotStatementListSyntax ParseStatementList()
        {
            var statement = Backtracking(it => it.ParseStatement(), false);

            if (statement == null)
                return new DotStatementListSyntax(Enumerable.Empty<(DotStatementSyntax, PunctuationSyntax?)>()
                    .ToList());

            var semicolonPunctuation = PeekToken<PunctuationSyntax>(SyntaxKind.SemicolonToken);
            if (semicolonPunctuation != null) NextToken();
            var statements = new List<(DotStatementSyntax, PunctuationSyntax?)> {(statement!, semicolonPunctuation)};

            while (true)
            {
                statement = Backtracking(it => it.ParseStatement(), false);
                if (statement == null) break;
                semicolonPunctuation = PeekToken<PunctuationSyntax>(SyntaxKind.SemicolonToken);
                if (semicolonPunctuation != null) NextToken();
                statements.Add((statement!, semicolonPunctuation));
            }

            return new DotStatementListSyntax(statements);
        }

        [return: MaybeNull]
        private DotAttributeListSyntax ParseAttributeList()
        {
            var attributes = new List<(PunctuationSyntax, DotAssignmentListSyntax?, PunctuationSyntax)>();
            var lsb = PeekToken<PunctuationSyntax>(SyntaxKind.LeftSquareBracketToken);
            NextToken();

            if (lsb == null)
            {
                Error($"Expecting [ in attribute list at {CurrentToken?.Start}.");
                return null;
            }

            var aList = Backtracking(it => it.ParseAssignmentList());
            var rsb = PeekToken<PunctuationSyntax>(SyntaxKind.RightSquareBracketToken);

            if (rsb == null)
            {
                Error($"Expecting ] in attribute list at {CurrentToken?.Start}.");
                return null;
            }

            NextToken();
            attributes.Add((lsb, aList, rsb));

            while (true)
            {
                lsb = PeekToken<PunctuationSyntax>(SyntaxKind.LeftSquareBracketToken);
                if (lsb != null) NextToken();
                if (lsb == null) break;
                aList = Backtracking(it => it.ParseAssignmentList());
                rsb = PeekToken<PunctuationSyntax>(SyntaxKind.RightSquareBracketToken);
                NextToken();

                if (rsb == null)
                {
                    Error($"Expecting ] in attribute list at {CurrentToken?.Start}.");
                    return null;
                }

                attributes.Add((lsb, aList, rsb));
            }

            return new DotAttributeListSyntax(attributes);
        }

        [return: MaybeNull]
        private DotAttributeStatementSyntax ParseAttributeStatement()
        {
            var graphNodeOrEdgeKeyword =
                PeekToken<KeywordSyntax>(SyntaxKind.GraphToken, SyntaxKind.NodeToken, SyntaxKind.EdgeToken);

            if (graphNodeOrEdgeKeyword == null) return null;
            NextToken();
            var attributeList = Backtracking(it => it.ParseAttributeList());

            return attributeList == null
                ? null
                : new DotAttributeStatementSyntax(graphNodeOrEdgeKeyword, attributeList);
        }

        [return: MaybeNull]
        private DotAssignmentListSyntax ParseAssignmentList()
        {
            var assignments = new List<(DotAssignmentSyntax, PunctuationSyntax?)>();
            var assignment = Backtracking(it => it.ParseAssignment());

            if (assignment == null)
            {
                Error($"Expecting assignment in assignment list at {CurrentToken?.Start}.");
                return null;
            }

            var commaOrSemicolon = PeekToken<PunctuationSyntax>(SyntaxKind.CommaToken, SyntaxKind.SemicolonToken);
            if (commaOrSemicolon != null) NextToken();
            assignments.Add((assignment, commaOrSemicolon));

            while (true)
            {
                assignment = Backtracking(it => it.ParseAssignment());
                if (assignment == null) break;
                commaOrSemicolon = PeekToken<PunctuationSyntax>(SyntaxKind.CommaToken, SyntaxKind.SemicolonToken);
                if (commaOrSemicolon != null) NextToken();
                assignments.Add((assignment, commaOrSemicolon));
            }

            return new DotAssignmentListSyntax(assignments);
        }

        [return: MaybeNull]
        private DotSubgraphSyntax ParseSubgraph()
        {
            (KeywordSyntax, DotIdSyntax)? subgraphGroup = null;
            var subgraphKeyword = PeekToken<KeywordSyntax>(SyntaxKind.SubgraphToken);
            if (subgraphKeyword != null) NextToken();
            var subgraphId = Backtracking(it => it.ParseId());
            if (subgraphKeyword == null && subgraphId != null) return null;
            if (subgraphKeyword != null && subgraphId != null) subgraphGroup = (subgraphKeyword, subgraphId);
            var lcbPunctuation = PeekToken<PunctuationSyntax>(SyntaxKind.LeftCurlyBracketToken);
            if (lcbPunctuation == null) return null;
            NextToken();
            var statementList = Backtracking(it => it.ParseStatementList());
            if (statementList == null) return null;
            var rcbPunctuation = PeekToken<PunctuationSyntax>(SyntaxKind.RightCurlyBracketToken);
            NextToken();

            return rcbPunctuation == null
                ? null
                : new DotSubgraphSyntax(subgraphGroup, lcbPunctuation, statementList, rcbPunctuation);
        }

        [return: MaybeNull]
        private DotIdSyntax ParseId()
        {
            var @string = PeekToken<StringSyntax>(
                SyntaxKind.StringToken,
                SyntaxKind.IdToken,
                SyntaxKind.NumberToken,
                SyntaxKind.HtmlStringToken);

            if (@string == null)
            {
                Error($"Expecting string or number in ID at {CurrentToken?.Start}.");
                return null;
            }

            NextToken();
            return new DotIdSyntax(@string);
        }

        [return: MaybeNull]
        private DotEdgeOperatorSyntax ParseEdgeOperator()
        {
            var edgeOpPunctuation =
                PeekToken<PunctuationSyntax>(SyntaxKind.EdgeOpTokenArrow, SyntaxKind.EdgeOpTokenBar);

            if (edgeOpPunctuation == null)
            {
                Error($"Expecting -- or -> at {CurrentToken?.Start}.");
                return null;
            }

            NextToken();
            return new DotEdgeOperatorSyntax(edgeOpPunctuation);
        }

        [return: MaybeNull]
        private DotAssignmentSyntax ParseAssignment()
        {
            var firstId = Backtracking(it => it.ParseId());

            if (firstId == null)
            {
                Error($"Expecting target key in assignment at {CurrentToken?.Start}.");
                return null;
            }

            var equalsPunctuation = PeekToken<PunctuationSyntax>(SyntaxKind.EqualsSignToken);

            if (equalsPunctuation == null)
            {
                Error($"Expecting = in assignment at {CurrentToken?.Start}.");
                return null;
            }

            NextToken();
            var secondId = Backtracking(it => it.ParseId());
            if (secondId != null) return new DotAssignmentSyntax(firstId, equalsPunctuation, secondId);
            Error($"Expecting value to assign in assignment at {CurrentToken?.Start}.");
            return null;
        }

        [return: MaybeNull]
        private DotNodeStatementSyntax ParseNodeStatement()
        {
            var nodeId = Backtracking(it => it.ParseNodeId());
            if (nodeId != null)
                return new DotNodeStatementSyntax(nodeId, Backtracking(it => it.ParseAttributeList()));
            Error($"Expecting node ID in node statement at {CurrentToken?.Start}.");
            return null;
        }

        [return: MaybeNull]
        private DotNodeIdSyntax ParseNodeId()
        {
            var id = Backtracking(it => it.ParseId());
            if (id != null) return new DotNodeIdSyntax(id, Backtracking(it => it.ParsePort()));
            Error($"Expecting ID in node ID at {CurrentToken?.Start}.");
            return null;
        }

        [return: MaybeNull]
        private DotPortSyntax ParsePort()
        {
            var firstColonPunctuation = PeekToken<PunctuationSyntax>(SyntaxKind.ColonToken);

            if (firstColonPunctuation == null)
            {
                Error($"Expecting : in port at {CurrentToken?.Start}.");
                return null;
            }

            NextToken();
            var firstId = Backtracking(it => it.ParseId());

            if (firstId == null)
            {
                Error($"Expecting ID after : at {CurrentToken?.Start}.");
                return null;
            }

            (PunctuationSyntax, DotIdSyntax)? secondGroup = null;
            var secondColonPunctuation = PeekToken<PunctuationSyntax>(SyntaxKind.ColonToken);
            if (secondColonPunctuation != null) NextToken();
            var secondId = Backtracking(it => it.ParseId());

            if (secondColonPunctuation != null && secondId != null)
                secondGroup = (secondColonPunctuation, secondId);

            return new DotPortSyntax(firstColonPunctuation, firstId, secondGroup);
        }

        [return: MaybeNull]
        private DotStatementSyntax ParseStatement()
        {
            DotEdgeStatementSyntax? edgeStatement;

            if (CurrentToken is StringSyntax ||
                PeekToken<PunctuationSyntax>(SyntaxKind.LeftCurlyBracketToken) != null)
                if (PeekTokenP1<PunctuationSyntax>(SyntaxKind.EdgeOpTokenArrow, SyntaxKind.EdgeOpTokenBar) != null)
                {
                    edgeStatement = Backtracking(it => it.ParseEdgeStatement(), false);
                    if (edgeStatement != null) return new DotStatementSyntax(edgeStatement);
                    Console.WriteLine($"Expecting edge statement at {CurrentToken?.Start}.");
                    return null;
                }

            edgeStatement = Backtracking(it => it.ParseEdgeStatement());
            if (edgeStatement != null) return new DotStatementSyntax(edgeStatement);

            if (CurrentToken is StringSyntax && PeekTokenP1<PunctuationSyntax>(SyntaxKind.EqualsSignToken) != null)
            {
                var assignment = Backtracking(it => it.ParseAssignment(), false);
                if (assignment != null) return new DotStatementSyntax(assignment);
                Error($"Expecting assignment statement at {CurrentToken?.Start}.");
                return null;
            }

            if (CurrentToken is StringSyntax)
            {
                var nodeStatement = Backtracking(it => it.ParseNodeStatement(), false);
                if (nodeStatement != null) return new DotStatementSyntax(nodeStatement);
                Error($"Expecting Node ID statement at {CurrentToken?.Start}.");
                return null;
            }

            if (PeekToken<PunctuationSyntax>(SyntaxKind.LeftCurlyBracketToken) != null ||
                PeekToken<KeywordSyntax>(SyntaxKind.SubgraphToken) != null)
            {
                var subgraph = Backtracking(it => it.ParseSubgraph(), false);
                if (subgraph != null) return new DotStatementSyntax(subgraph);
                Error($"Expecting subgraph statement at {CurrentToken?.Start}.");
                return null;
            }

            if (PeekToken<KeywordSyntax>(SyntaxKind.NodeToken, SyntaxKind.GraphToken, SyntaxKind.EdgeToken) != null)
            {
                var attributeStatement = Backtracking(it => it.ParseAttributeStatement(), false);
                if (attributeStatement != null) return new DotStatementSyntax(attributeStatement);
                Error($"Expecting attribute statement at {CurrentToken?.Start}.");
                return null;
            }

            Error($"Expecting statement at {CurrentToken?.Start}.");
            return null;
        }

        [return: MaybeNull]
        private DotEdgeRightHandSideSyntax ParseEdgeRightHandSide()
        {
            var edgeOperator = Backtracking(it => it.ParseEdgeOperator());

            if (edgeOperator == null)
            {
                Error($"Expecting edge operator in edge RHS at {CurrentToken?.Start}.");
                return null;
            }

            var nodeIdOrSubgraph =
                Backtracking(it => it.ParseNodeId()) ?? (DotSyntax?) Backtracking(it => it.ParseSubgraph());

            if (nodeIdOrSubgraph == null)
            {
                Error(
                    $"Expecting node ID or subgraph after edge operator in edge RHS at {CurrentToken?.Start}.");
                return null;
            }

            var edges = new List<(DotEdgeOperatorSyntax, DotSyntax)> {(edgeOperator, nodeIdOrSubgraph)};

            while (true)
            {
                edgeOperator = Backtracking(it => it.ParseEdgeOperator());
                if (edgeOperator == null) break;

                nodeIdOrSubgraph = Backtracking(it => it.ParseNodeId()) ??
                                   (DotSyntax?) Backtracking(it => it.ParseSubgraph());

                if (nodeIdOrSubgraph != null)
                {
                    edges.Add((edgeOperator, nodeIdOrSubgraph));
                    continue;
                }

                Error($"Expecting node ID or subgraph after edge operator at {CurrentToken?.Start}.");
                return null;
            }

            return new DotEdgeRightHandSideSyntax(edges);
        }

        [return: MaybeNull]
        private DotEdgeStatementSyntax ParseEdgeStatement()
        {
            var nodeIdOrSubgraph =
                Backtracking(it => it.ParseNodeId()) ?? (DotSyntax?) Backtracking(it => it.ParseSubgraph());

            if (nodeIdOrSubgraph == null)
            {
                Error($"Expecting node ID or subgraph in edge statement at {CurrentToken?.Start}.");
                return null;
            }

            var edgeRhs = Backtracking(it => it.ParseEdgeRightHandSide());

            if (edgeRhs != null)
                return new DotEdgeStatementSyntax(nodeIdOrSubgraph, edgeRhs,
                    Backtracking(it => it.ParseAttributeList()));

            Error($"Expecting edge RHS at {CurrentToken?.Start}.");
            return null;
        }

        [DebuggerStepThrough]
        [return: MaybeNull]
        private TResult? Backtracking<TResult>([DisallowNull] Func<Parser, TResult?> action, bool hideErrors = true)
            where TResult : DotSyntax
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var old = _currentTokenOffset;
            var oldErrors = _errors.ToList();
            var result = action(this);
            if (result != null) return result;
            _currentTokenOffset = old;
            if (hideErrors) _errors = oldErrors;
            return null;
        }

        [DebuggerStepThrough]
        [return: MaybeNull]
        private T PeekToken<T>([DisallowNull] params SyntaxKind[] kind) where T : SyntaxToken
        {
            if (kind == null) throw new ArgumentNullException(nameof(kind));
            T? result;
            var raw = CurrentToken;
            if (raw == null) return null;

            if (!kind.Contains(raw.Kind) || !(raw is T)) result = null;
            else result = (T) raw;

            return result;
        }

        private void Error([DisallowNull] string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            _errors.Add((message, CurrentToken));
        }

        [DebuggerStepThrough]
        [return: MaybeNull]
        private T PeekTokenP1<T>([DisallowNull] params SyntaxKind[] kind) where T : SyntaxToken
        {
            if (kind == null) throw new ArgumentNullException(nameof(kind));
            T? result;
            var raw = _tokens.ElementAtOrDefault(_currentTokenOffset + 1);
            if (raw == null) return null;

            if (!kind.Contains(raw.Kind) || !(raw is T)) result = null;
            else result = (T) raw;

            return result;
        }

        [DebuggerStepThrough]
        private void NextToken() => _currentTokenOffset++;
    }
}