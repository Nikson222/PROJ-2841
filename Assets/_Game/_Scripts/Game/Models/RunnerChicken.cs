namespace _Game._Scripts.Game.Models
{
    public class RunnerChicken
    {
        public int Id { get; }
        public ChickenColor Color { get; }
        public float Progress { get; set; }
        public bool IsGrabbed { get; set; }
        public bool HasReachedEnd { get; set; }
        public bool IsDetachedFromTrack { get; set; }

        public RunnerChicken(int id, ChickenColor color)
        {
            Id = id;
            Color = color;
            Progress = 0f;
            IsGrabbed = false;
            HasReachedEnd = false;
            IsDetachedFromTrack = false;
        }
    }
}