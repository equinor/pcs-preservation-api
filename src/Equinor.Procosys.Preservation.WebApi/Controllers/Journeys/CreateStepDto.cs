﻿namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class CreateStepDto
    {
        public string Title { get; set; }
        public int ModeId { get; set; }
        public string ResponsibleCode { get; set; }
        public bool TransferOnRfccSign { get; set; }
        public bool TransferOnRfocSign { get; set; }
    }
}
