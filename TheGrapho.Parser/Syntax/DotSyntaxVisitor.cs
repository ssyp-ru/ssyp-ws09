// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public abstract class DotSyntaxVisitor<TResult>
    {
        [return: MaybeNull]
        public virtual TResult Visit([DisallowNull] DotSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            return syntax.Accept(this);
        }

        [return: MaybeNull]
        public virtual TResult VisitAssignmentList([DisallowNull] DotAssignmentListSyntax assignmentList)
        {
            if (assignmentList == null) throw new ArgumentNullException(nameof(assignmentList));
            return DefaultVisit(assignmentList);
        }

        [return: MaybeNull]
        public TResult DefaultVisit([DisallowNull] DotSyntax syntax) => default;

        [return: MaybeNull]
        public virtual TResult VisitAssignment([DisallowNull] DotAssignmentSyntax assignment)
        {
            if (assignment == null) throw new ArgumentNullException(nameof(assignment));
            return DefaultVisit(assignment);
        }

        [return: MaybeNull]
        public virtual TResult VisitAttributeList([DisallowNull] DotAttributeListSyntax attributeList)
        {
            if (attributeList == null) throw new ArgumentNullException(nameof(attributeList));
            return DefaultVisit(attributeList);
        }

        [return: MaybeNull]
        public virtual TResult VisitAttributeStatement([DisallowNull] DotAttributeStatementSyntax attributeStatement)
        {
            if (attributeStatement == null) throw new ArgumentNullException(nameof(attributeStatement));
            return DefaultVisit(attributeStatement);
        }

        [return: MaybeNull]
        public virtual TResult VisitEdgeOperator([DisallowNull] DotEdgeOperatorSyntax edgeOperator)
        {
            if (edgeOperator == null) throw new ArgumentNullException(nameof(edgeOperator));
            return DefaultVisit(edgeOperator);
        }

        [return: MaybeNull]
        public virtual TResult VisitEdgeRightHandSide([DisallowNull] DotEdgeRightHandSideSyntax edgeRhs)
        {
            if (edgeRhs == null) throw new ArgumentNullException(nameof(edgeRhs));
            return DefaultVisit(edgeRhs);
        }

        [return: MaybeNull]
        public virtual TResult VisitEdgeStatement([DisallowNull] DotEdgeStatementSyntax edgeStatement)
        {
            if (edgeStatement == null) throw new ArgumentNullException(nameof(edgeStatement));
            return DefaultVisit(edgeStatement);
        }

        [return: MaybeNull]
        public virtual TResult VisitGraph([DisallowNull] DotGraphSyntax graph)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            return DefaultVisit(graph);
        }

        [return: MaybeNull]
        public virtual TResult VisitId([DisallowNull] DotIdSyntax id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return DefaultVisit(id);
        }

        [return: MaybeNull]
        public virtual TResult VisitNodeId([DisallowNull] DotNodeIdSyntax nodeId)
        {
            if (nodeId == null) throw new ArgumentNullException(nameof(nodeId));
            return DefaultVisit(nodeId);
        }

        [return: MaybeNull]
        public virtual TResult VisitNodeStatement([DisallowNull] DotNodeStatementSyntax nodeStatement)
        {
            if (nodeStatement == null) throw new ArgumentNullException(nameof(nodeStatement));
            return DefaultVisit(nodeStatement);
        }

        [return: MaybeNull]
        public virtual TResult VisitPort([DisallowNull] DotPortSyntax port)
        {
            if (port == null) throw new ArgumentNullException(nameof(port));
            return DefaultVisit(port);
        }

        [return: MaybeNull]
        public virtual TResult VisitStatementList([DisallowNull] DotStatementListSyntax statementList)
        {
            if (statementList == null) throw new ArgumentNullException(nameof(statementList));
            return DefaultVisit(statementList);
        }

        [return: MaybeNull]
        public virtual TResult VisitStatement([DisallowNull] DotStatementSyntax statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return DefaultVisit(statement);
        }

        [return: MaybeNull]
        public virtual TResult VisitSubgraph([DisallowNull] DotSubgraphSyntax subgraph)
        {
            if (subgraph == null) throw new ArgumentNullException(nameof(subgraph));
            return DefaultVisit(subgraph);
        }
    }
}