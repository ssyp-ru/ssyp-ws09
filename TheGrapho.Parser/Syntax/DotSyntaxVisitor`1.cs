// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace TheGrapho.Parser.Syntax
{
    public abstract class DotSyntaxVisitor
    {
        public virtual void Visit([DisallowNull] DotSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            syntax.Accept(this);
        }

        public virtual void VisitAssignmentList([DisallowNull] DotAssignmentListSyntax? assignmentList)
        {
            if (assignmentList == null) throw new ArgumentNullException(nameof(assignmentList));
            DefaultVisit(assignmentList);
        }

        protected virtual void DefaultVisit([DisallowNull] DotSyntax syntax)
        {
        }

        public virtual void VisitAssignment([DisallowNull] DotAssignmentSyntax assignment)
        {
            if (assignment == null) throw new ArgumentNullException(nameof(assignment));
            DefaultVisit(assignment);
        }

        public virtual void VisitAttributeList([DisallowNull] DotAttributeListSyntax attributeList)
        {
            if (attributeList == null) throw new ArgumentNullException(nameof(attributeList));
            DefaultVisit(attributeList);
        }

        public virtual void VisitAttributeStatement([DisallowNull] DotAttributeStatementSyntax attributeStatement)
        {
            if (attributeStatement == null) throw new ArgumentNullException(nameof(attributeStatement));
            DefaultVisit(attributeStatement);
        }

        public virtual void VisitEdgeOperator([DisallowNull] DotEdgeOperatorSyntax edgeOperator)
        {
            if (edgeOperator == null) throw new ArgumentNullException(nameof(edgeOperator));
            DefaultVisit(edgeOperator);
        }

        public virtual void VisitEdgeRightHandSide([DisallowNull] DotEdgeRightHandSideSyntax edgeRhs)
        {
            if (edgeRhs == null) throw new ArgumentNullException(nameof(edgeRhs));
            DefaultVisit(edgeRhs);
        }

        public virtual void VisitEdgeStatement([DisallowNull] DotEdgeStatementSyntax edgeStatement)
        {
            if (edgeStatement == null) throw new ArgumentNullException(nameof(edgeStatement));
            DefaultVisit(edgeStatement);
        }

        public virtual void VisitGraph([DisallowNull] DotGraphSyntax graph)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            DefaultVisit(graph);
        }

        public virtual void VisitId([DisallowNull] DotIdSyntax id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            DefaultVisit(id);
        }

        public virtual void VisitNodeId([DisallowNull] DotNodeIdSyntax nodeId)
        {
            if (nodeId == null) throw new ArgumentNullException(nameof(nodeId));
            DefaultVisit(nodeId);
        }

        public virtual void VisitNodeStatement([DisallowNull] DotNodeStatementSyntax nodeStatement)
        {
            if (nodeStatement == null) throw new ArgumentNullException(nameof(nodeStatement));
            DefaultVisit(nodeStatement);
        }

        public virtual void VisitPort([DisallowNull] DotPortSyntax port)
        {
            if (port == null) throw new ArgumentNullException(nameof(port));
            DefaultVisit(port);
        }

        public virtual void VisitStatementList([DisallowNull] DotStatementListSyntax statementList)
        {
            if (statementList == null) throw new ArgumentNullException(nameof(statementList));
            DefaultVisit(statementList);
        }

        public virtual void VisitStatement([DisallowNull] DotStatementSyntax statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            DefaultVisit(statement);
        }

        public virtual void VisitSubgraph([DisallowNull] DotSubgraphSyntax subgraph)
        {
            if (subgraph == null) throw new ArgumentNullException(nameof(subgraph));
            DefaultVisit(subgraph);
        }
    }
}