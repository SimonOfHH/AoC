using System.Diagnostics;
using System.Text;

namespace AdventOfCode;

public class Day_10 : BaseDay
{
    private readonly bool Sample = false;
    private VideoSystem videoSystem;
    public Day_10()
    {
        videoSystem = ParseInput();
        videoSystem.ProcessCpuInstructions();
    }

    public override ValueTask<string> Solve_1() => new(videoSystem.CPU.SignalStrengths.Sum().ToString());

    public override ValueTask<string> Solve_2() => new(videoSystem.CRT.GetOutput());

    private VideoSystem ParseInput()
    {
        return new VideoSystem(this.GetInput(Sample));
    }
}

class VideoSystem
{
    public CPU CPU { get; set; }
    public CRT CRT { get; set; }
    public VideoSystem(string[] input)
    {
        CPU = new CPU();
        CRT = new CRT();
        foreach (var s in input.Reverse())
            CPU.Instructions.Push(new CpuInstruction(s));
    }
    public void ProcessCpuInstructions()
    {
        while ((CPU.Instructions.Count > 0) || (CPU.ProcessingInstruction != null))
            CPU.Cycle(CRT);
    }
}
class CRT
{
    public const int Width = 40;
    public Dictionary<int, string> VideoBuffer { get; set; }
    public string CurrentSpriteValue => CurrentSprite();
    public List<int> SpriteRange => Enumerable.Range(CpuStackValue - 1, 3).ToList();
    public int CurrentlyDrawnPixel => VideoBuffer.Sum(x => x.Value.Length) % Width;
    public int CpuStackValue { get; set; }
    [DebuggerStepThroughAttribute]
    public CRT()
    {
        VideoBuffer = new Dictionary<int, string>();
        VideoBuffer.Add(0, "");
    }
    private string CurrentSprite()
    {
        return String.Join("", Enumerable.Range(0, Width).ToList().Select(i => SpriteRange.Contains(i) ? "#" : ".").ToList());
    }
    public void Cycle(int cpuStackValue)
    {
        CpuStackValue = cpuStackValue;
        AddToVideoBuffer(CurrentSpriteValue[CurrentlyDrawnPixel].ToString());
    }
    public string GetOutput()
    {
        var builder = new StringBuilder();
        VideoBuffer.ToList().ForEach(x => builder.AppendLine(x.Value));
        return builder.ToString();
    }
    private void AddToVideoBuffer(string pixel)
    {
        VideoBuffer[VideoBuffer.Count - 1] += pixel;
        if (VideoBuffer.Last().Value.Length == Width)
            VideoBuffer.Add(VideoBuffer.Count, String.Empty);
    }
}
class CPU
{
    public CpuInstruction ProcessingInstruction { get; set; }
    public Stack<CpuInstruction> Instructions { get; set; }
    public Dictionary<string, int> Register { get; set; }
    public List<int> SignalStrengths { get; set; }
    public int StackValueX => Register["x"];
    public int PreviousStackValue { get; set; }
    private int CycleCount { get; set; }
    [DebuggerStepThroughAttribute]
    public CPU()
    {
        Instructions = new Stack<CpuInstruction>();
        Register = new Dictionary<string, int>();
        Register.Add("x", 1);
        SignalStrengths = new List<int>();
    }
    public void Cycle(CRT crt)
    {
        CycleCount++;
        if ((CycleCount == 20) || ((CycleCount - 20) % 40 == 0))
            SignalStrengths.Add(CycleCount * Register["x"]);

        if (ProcessingInstruction != null)
            FinishExecution(crt);
        else
        {
            BeginExecution(crt);
            if (ProcessingInstruction.Action == CpuInstructionAction.noop)
                FinishExecution(crt);
        }
    }
    private void BeginExecution(CRT crt)
    {
        ProcessingInstruction = Instructions.Pop();
        if (ProcessingInstruction.Action != CpuInstructionAction.noop)
            crt.Cycle(StackValueX);
    }
    private void FinishExecution(CRT crt)
    {
        crt.Cycle(StackValueX);
        if (ProcessingInstruction.Action == CpuInstructionAction.addx)
            Register["x"] = Register["x"] + ProcessingInstruction.Value;

        ProcessingInstruction = null;
    }
}
class CpuInstruction
{
    public CpuInstructionAction Action { get; set; }
    public int Value { get; set; }
    public CpuInstruction(string s)
    {
        Action = (CpuInstructionAction)Enum.Parse(typeof(CpuInstructionAction), s.Split(" ")[0].Trim());
        if (Action != CpuInstructionAction.noop)
            Value = int.Parse(s.Split(" ")[1].Trim());
    }
    public override string ToString()
    {
        return String.Format("{0} {1}", Action, Value);
    }
}
enum CpuInstructionAction
{
    noop,
    addx
}