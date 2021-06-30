using System;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public static class ShiftedGherkinFileBlockExtensions
    {
        public static IGherkinFileBlock Shift(this IGherkinFileBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            if (fileBlock is IInvalidFileBlock)
                return Shift((IInvalidFileBlock)fileBlock, lineShift);
            if (fileBlock is IBackgroundBlock)
                return Shift((IBackgroundBlock)fileBlock, lineShift);
            if (fileBlock is IScenarioOutlineBlock)
                return Shift((IScenarioOutlineBlock)fileBlock, lineShift);
            if (fileBlock is IScenarioBlock)
                return Shift((IScenarioBlock)fileBlock, lineShift);

            // we cannot shift header block
            throw new NotSupportedException("block type not supported: " + fileBlock.GetType());
        }

        public static IBackgroundBlock Shift(this IBackgroundBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            UnWrapShiftdFileBlock(ref fileBlock, ref lineShift);
            return new ShiftedBackgroundBlock(fileBlock, lineShift);
        }

        public static IInvalidFileBlock Shift(this IInvalidFileBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            UnWrapShiftdFileBlock(ref fileBlock, ref lineShift);
            return new ShiftedInvalidFileBlock(fileBlock, lineShift);
        }

        public static IScenarioBlock Shift(this IScenarioBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            if (fileBlock is IScenarioOutlineBlock)
                return Shift((IScenarioOutlineBlock)fileBlock, lineShift);

            UnWrapShiftdFileBlock(ref fileBlock, ref lineShift);
            return new ShiftedScenarioBlock(fileBlock, lineShift);
        }

        public static IScenarioOutlineBlock Shift(this IScenarioOutlineBlock fileBlock, int lineShift)
        {
            if (fileBlock == null) throw new ArgumentNullException("fileBlock");

            UnWrapShiftdFileBlock(ref fileBlock, ref lineShift);
            return new ShiftedScenarioOutlineBlock(fileBlock, lineShift);
        }

        private static void UnWrapShiftdFileBlock<T>(ref T fileBlock, ref int lineShift) where T : IGherkinFileBlock
        {
            ShiftedGherkinFileBlock<T> shiftedGherkinFileBlock = fileBlock as ShiftedGherkinFileBlock<T>;
            if (shiftedGherkinFileBlock != null)
            {
                // if it was already shifted, we change the wrapper only
                fileBlock = shiftedGherkinFileBlock.BaseBlock;
                lineShift += shiftedGherkinFileBlock.LineShift;
            }
        }
    }
}