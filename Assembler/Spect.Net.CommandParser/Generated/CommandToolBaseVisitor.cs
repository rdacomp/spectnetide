﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6.6
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Users\dotne\source\repos\spectnetide\Assembler\AntlrCommandParserGenerator\AntlrCommandParserGenerator\CommandTool.g4 by ANTLR 4.6.6

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Spect.Net.CommandParser.Generated {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="ICommandToolVisitor{Result}"/>,
/// which can be extended to create a visitor which only needs to handle a subset
/// of the available methods.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.6")]
[System.CLSCompliant(false)]
public partial class CommandToolBaseVisitor<Result> : AbstractParseTreeVisitor<Result>, ICommandToolVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.compileUnit"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitCompileUnit([NotNull] CommandToolParser.CompileUnitContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.toolCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitToolCommand([NotNull] CommandToolParser.ToolCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.gotoCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitGotoCommand([NotNull] CommandToolParser.GotoCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.gotoSymbolCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitGotoSymbolCommand([NotNull] CommandToolParser.GotoSymbolCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.romPageCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitRomPageCommand([NotNull] CommandToolParser.RomPageCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.bankPageCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitBankPageCommand([NotNull] CommandToolParser.BankPageCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.memModeCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitMemModeCommand([NotNull] CommandToolParser.MemModeCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.labelCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitLabelCommand([NotNull] CommandToolParser.LabelCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.commentCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitCommentCommand([NotNull] CommandToolParser.CommentCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.prefixCommentCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitPrefixCommentCommand([NotNull] CommandToolParser.PrefixCommentCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.setBreakpointCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSetBreakpointCommand([NotNull] CommandToolParser.SetBreakpointCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.toggleBreakpointCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitToggleBreakpointCommand([NotNull] CommandToolParser.ToggleBreakpointCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.removeBreakpointCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitRemoveBreakpointCommand([NotNull] CommandToolParser.RemoveBreakpointCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.updateBreakpointCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitUpdateBreakpointCommand([NotNull] CommandToolParser.UpdateBreakpointCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.eraseAllBreakpointsCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitEraseAllBreakpointsCommand([NotNull] CommandToolParser.EraseAllBreakpointsCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.retrieveCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitRetrieveCommand([NotNull] CommandToolParser.RetrieveCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.literalCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitLiteralCommand([NotNull] CommandToolParser.LiteralCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.disassemblyTypeCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitDisassemblyTypeCommand([NotNull] CommandToolParser.DisassemblyTypeCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.reDisassemblyCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitReDisassemblyCommand([NotNull] CommandToolParser.ReDisassemblyCommandContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="CommandToolParser.jumpCommand"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitJumpCommand([NotNull] CommandToolParser.JumpCommandContext context) { return VisitChildren(context); }
}
} // namespace Spect.Net.CommandParser.Generated
