namespace ScalpingMO.Analysis.Analysis.API.Models
{
    public abstract class BaseModelRequest
    {
        public abstract List<string> Errors { get; protected set; }
        public abstract bool IsValid();
        public abstract void Validate();
    }
}
