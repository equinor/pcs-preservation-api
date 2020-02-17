﻿using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateAreaTagDto
    {
        public string ToDo { get; set; }
        public string ProjectName { get; set; }
        public int StepId { get; set; }
        public IEnumerable<TagRequirementDto> Requirements { get; set; }
        public string Remark { get; set; }
    }
}
