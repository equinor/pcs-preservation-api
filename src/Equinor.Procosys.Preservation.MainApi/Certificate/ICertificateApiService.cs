﻿using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.MainApi.Certificate
{
    public interface ICertificateApiService
    {
        Task<ProcosysCertificateTagsModel> TryGetCertificateTagsAsync(
            string plant, 
            string projectName,
            string certificateNo,
            string certificateType);
    }
}