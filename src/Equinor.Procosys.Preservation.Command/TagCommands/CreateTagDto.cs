﻿namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagDto
    {
        public string TagNo { get; set; }
        public string ProjectNo { get; set; }
        public int JourneyId { get; set; }
        public int StepId { get; set; }
    }
}
