// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace TheGrapho.Parser.Syntax
{
    public enum SyntaxKind : byte
    {
        Nothing,

        // Trivia
        WhitespaceTrivia,
        PreprocessorTrivia,
        BlockCommentTrivia,
        LineCommentTrivia,

        // Keywords
        StrictToken,
        GraphToken,
        DigraphToken,
        NodeToken,
        EdgeToken,
        SubgraphToken,

        // IDs
        StringToken,
        NumberToken,
        IdToken,
        HtmlStringToken,

        // Punctuation
        SemicolonToken,
        LeftSquareBracketToken,
        RightSquareBracketToken,
        EqualsSignToken,
        LeftCurlyBracketToken,
        RightCurlyBracketToken,
        EdgeOpTokenBar,
        EdgeOpTokenArrow,
        ColonToken,
        CommaToken,

        // DOT Syntax
        DotGraph,
        DotStatementList,
        DotStatement,
        DotAttributeStatement,
        DotAttributeList,
        DotAssignmentList,
        DotEdgeStatement,
        DotEdgeRightHandSide,
        DotEdgeOperator,
        DotNodeStatement,
        DotNodeId,
        DotPort,
        DotSubgraph,
        DotId,
        DotAssignment,
        DotAssignmentOrDeclaration
    }
}