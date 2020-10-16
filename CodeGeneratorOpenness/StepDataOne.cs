using System.Collections.Generic;

namespace CodeGeneratorOpenness
{
    public class StepDataOne
    {
        // some fixed assignements:
        // step   1 is always the first step (initial)
        // step 100 is always a "step end"
        // we can work only step 1 to 100
        //
        // later this will come from the config file

        public List<OneStep> Steps = new List<OneStep>();

        // one step sequence for the first test
        public StepDataOne()
        {
            OneStep one = new OneStep();
            one.Number = 1;
            one.Description = "Step 1";
            one.NextStep = 21;
            one.AbortStep = 0;
            one.OptionStep = 0;

            one.WaitingTime = 0;
            one.MonitorTime = 0;
            Steps.Add(one);

            one = new OneStep();
            one.Number = 21;
            one.Description = "Step 21";
            one.NextStep = 27;
            one.AbortStep = 32;
            one.OptionStep = 43;

            one.WaitingTime = 0;
            one.MonitorTime = 0;
            Steps.Add(one);

            one = new OneStep();
            one.Number = 27;
            one.Description = "Step 27";
            one.NextStep = 32;
            one.AbortStep = 0;
            one.OptionStep = 0;

            one.WaitingTime = 0;
            one.MonitorTime = 0;
            Steps.Add(one);

            one = new OneStep();
            one.Number = 32;
            one.Description = "Step 32";
            one.NextStep = 43;
            one.AbortStep = 0;
            one.OptionStep = 0;

            one.WaitingTime = 0;
            one.MonitorTime = 0;
            Steps.Add(one);

            one = new OneStep();
            one.Number = 43;
            one.Description = "Step 43";
            one.NextStep = 100; // step seq. with end
            one.NextStep = 1;   // loop step seq. with jump
            one.AbortStep = 0;
            one.OptionStep = 0;

            one.WaitingTime = 0;
            one.MonitorTime = 0;
            Steps.Add(one);    
        }

       
    }

    public class OneStep
    {
        public int Number;
        public string Description;
        public int NextStep;
        public int AbortStep;
        public int OptionStep;

        public int WaitingTime;
        public int MonitorTime;

    }
}
