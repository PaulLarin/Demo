namespace TaskListCommander.Model
{
    public class ProgressInfo
    {
        public ProgressInfo(TaskState state, int percents)
        {
            State = state;
            Percents = percents;
        }

        public TaskState State { get; }
        public int Percents { get; }
    }
}
