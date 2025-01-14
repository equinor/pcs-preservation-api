using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetJourneyById
{
    public class JourneyDetailsDto
    {
        public JourneyDetailsDto(int id, string title, bool isInUse, bool isVoided, IEnumerable<StepDetailsDto> steps, JourneyProjectDetailsDto journeyProjectDetailsDto , string rowVersion)
        {
            Id = id;
            Title = title;
            IsInUse = isInUse;
            IsVoided = isVoided;
            Steps = steps;
            RowVersion = rowVersion;
            Project = journeyProjectDetailsDto;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public bool IsInUse { get; }
        public IEnumerable<StepDetailsDto> Steps { get; }
        public string RowVersion { get; }
        public JourneyProjectDetailsDto Project { get; set; }
        public class JourneyProjectDetailsDto
        {
            public JourneyProjectDetailsDto(int id, string name, string description)
            {
                Id = id;
                Name = name;
                Description = description;
            }
            public int Id {get ; set;}
            public string Name {get ; set;}
            public string Description {get ; set;}
        }
    }
}
