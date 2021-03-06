//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6.6
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Users\dotne\source\repos\spectnetide\v2\Assembler\AntlrZ80TestParserGenerator\AntlrZ80TestParserGenerator\Z80TestParser.g4 by ANTLR 4.6.6

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Spect.Net.TestParser.Generated {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="Z80TestParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.6")]
[System.CLSCompliant(false)]
public interface IZ80TestParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.compileUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCompileUnit([NotNull] Z80TestParser.CompileUnitContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.testSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTestSet([NotNull] Z80TestParser.TestSetContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.sp48Mode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSp48Mode([NotNull] Z80TestParser.Sp48ModeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.sourceContext"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSourceContext([NotNull] Z80TestParser.SourceContextContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.callstub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCallstub([NotNull] Z80TestParser.CallstubContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.testOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTestOptions([NotNull] Z80TestParser.TestOptionsContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.testOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTestOption([NotNull] Z80TestParser.TestOptionContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.dataBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDataBlock([NotNull] Z80TestParser.DataBlockContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.dataBlockBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDataBlockBody([NotNull] Z80TestParser.DataBlockBodyContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.valueDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitValueDef([NotNull] Z80TestParser.ValueDefContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.memPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemPattern([NotNull] Z80TestParser.MemPatternContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.memPatternBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemPatternBody([NotNull] Z80TestParser.MemPatternBodyContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.byteSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitByteSet([NotNull] Z80TestParser.ByteSetContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.wordSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWordSet([NotNull] Z80TestParser.WordSetContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.text"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitText([NotNull] Z80TestParser.TextContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.portMock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPortMock([NotNull] Z80TestParser.PortMockContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.portPulse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPortPulse([NotNull] Z80TestParser.PortPulseContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.initSettings"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInitSettings([NotNull] Z80TestParser.InitSettingsContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.setupCode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSetupCode([NotNull] Z80TestParser.SetupCodeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.cleanupCode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCleanupCode([NotNull] Z80TestParser.CleanupCodeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.invokeCode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInvokeCode([NotNull] Z80TestParser.InvokeCodeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.testBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTestBlock([NotNull] Z80TestParser.TestBlockContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.testParams"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTestParams([NotNull] Z80TestParser.TestParamsContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.testCase"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTestCase([NotNull] Z80TestParser.TestCaseContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.arrange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrange([NotNull] Z80TestParser.ArrangeContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignment([NotNull] Z80TestParser.AssignmentContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.regAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRegAssignment([NotNull] Z80TestParser.RegAssignmentContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.flagStatus"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFlagStatus([NotNull] Z80TestParser.FlagStatusContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.memAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemAssignment([NotNull] Z80TestParser.MemAssignmentContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.act"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAct([NotNull] Z80TestParser.ActContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.breakpoint"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBreakpoint([NotNull] Z80TestParser.BreakpointContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.assert"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssert([NotNull] Z80TestParser.AssertContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.reg8"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReg8([NotNull] Z80TestParser.Reg8Context context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.reg8Idx"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReg8Idx([NotNull] Z80TestParser.Reg8IdxContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.reg8Spec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReg8Spec([NotNull] Z80TestParser.Reg8SpecContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.reg16"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReg16([NotNull] Z80TestParser.Reg16Context context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.reg16Idx"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReg16Idx([NotNull] Z80TestParser.Reg16IdxContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.reg16Spec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReg16Spec([NotNull] Z80TestParser.Reg16SpecContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.flag"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFlag([NotNull] Z80TestParser.FlagContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpr([NotNull] Z80TestParser.ExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.orExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOrExpr([NotNull] Z80TestParser.OrExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.xorExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitXorExpr([NotNull] Z80TestParser.XorExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.andExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAndExpr([NotNull] Z80TestParser.AndExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.equExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEquExpr([NotNull] Z80TestParser.EquExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.relExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRelExpr([NotNull] Z80TestParser.RelExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.shiftExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitShiftExpr([NotNull] Z80TestParser.ShiftExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.addExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAddExpr([NotNull] Z80TestParser.AddExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.multExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMultExpr([NotNull] Z80TestParser.MultExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.unaryExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnaryExpr([NotNull] Z80TestParser.UnaryExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.literalExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteralExpr([NotNull] Z80TestParser.LiteralExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.symbolExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSymbolExpr([NotNull] Z80TestParser.SymbolExprContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.registerSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRegisterSpec([NotNull] Z80TestParser.RegisterSpecContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.addrSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAddrSpec([NotNull] Z80TestParser.AddrSpecContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.reachSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReachSpec([NotNull] Z80TestParser.ReachSpecContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.memReadSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemReadSpec([NotNull] Z80TestParser.MemReadSpecContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="Z80TestParser.memWriteSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemWriteSpec([NotNull] Z80TestParser.MemWriteSpecContext context);
}
} // namespace Spect.Net.TestParser.Generated
