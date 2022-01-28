﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Equinor.ProCoSys.Preservation.Command.TagCommands.AutoTransfer;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Telemetry;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class CertificateEventProcessorService : ICertificateEventProcessorService
    {
        private readonly Guid _preservationApiOid;

        private readonly ILogger _logger;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IMediator _mediator;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IPlantSetter _plantSetter;
        private readonly ICurrentUserSetter _currentUserSetter;
        private readonly IClaimsTransformation _claimsTransformation;
        private readonly IAuthenticator _authenticator;

        private const string PreservationBusReceiverTelemetryEvent = "Preservation Bus Receiver";

        public CertificateEventProcessorService(
            ILogger<CertificateEventProcessorService> logger,
            ITelemetryClient telemetryClient,
            IMediator mediator,
            IClaimsProvider claimsProvider,
            IPlantSetter plantSetter,
            ICurrentUserSetter currentUserSetter,
            IClaimsTransformation claimsTransformation,
            IAuthenticator authenticator,
            IOptionsSnapshot<AuthenticatorOptions> authenticatorOptions
        )
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _mediator = mediator;
            _claimsProvider = claimsProvider;
            _currentUserSetter = currentUserSetter;
            _claimsTransformation = claimsTransformation;
            _plantSetter = plantSetter;
            _authenticator = authenticator;
            _preservationApiOid = authenticatorOptions.Value.PreservationApiObjectId;
        }

        public async Task ProcessCertificateEvent(string messageJson)
        {
            var certificateEvent = JsonSerializer.Deserialize<Equinor.ProCoSys.Preservation.WebApi.Synchronization.CertificateTopic>(messageJson);

            if (certificateEvent != null && (
                certificateEvent.Plant.IsEmpty() ||
                certificateEvent.ProjectName.IsEmpty() ||
                certificateEvent.CertificateNo.IsEmpty()))
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid CertificateEvent {messageJson}");
            }

            TrackCertificateEvent(certificateEvent);

            await HandleAutoTransferIfRelevant(certificateEvent);
        }

        private async Task HandleAutoTransferIfRelevant(CertificateTopic certificateEvent)
        {

            if (certificateEvent.CertificateType == "RFOC" || certificateEvent.CertificateType == "RFCC")
            {
                await SetUserContext(certificateEvent.Plant);
                var result = await _mediator.Send(new AutoTransferCommand(certificateEvent.ProjectName, certificateEvent.CertificateNo, certificateEvent.CertificateType.ToString()));

                LogAutoTransferResult(certificateEvent, result);
               
            }
        }

        private async Task SetUserContext(string plant)
        {
            _authenticator.AuthenticationType = AuthenticationType.AsApplication;
            _currentUserSetter.SetCurrentUserOid(_preservationApiOid);
            var currentUser = _claimsProvider.GetCurrentUser();

            _plantSetter.SetPlant(plant);
            await _claimsTransformation.TransformAsync(currentUser);
        }

        private void LogAutoTransferResult(CertificateTopic certificateEvent, Result<Unit> result)
        {
            var resultOk = result.ResultType == ResultType.Ok;

            _logger.LogInformation(resultOk ? "Autotransfer tags complete." : "Autotransfer tags functions failed.");

            var telemetryDictionary = new Dictionary<string, string>
            {
                {"Status", resultOk ? "Succeeded" : "Failed"},
                {"Plant", certificateEvent.Plant},
                {"Type", "Autotransfer tags"},
                {"ProjectName", certificateEvent.ProjectName},
                {"CertificateNo", certificateEvent.CertificateNo},
                {"CertificateType", certificateEvent.CertificateType.ToString()}
            };

            _telemetryClient.TrackEvent("Synchronization Status", telemetryDictionary);
        }

        private void TrackCertificateEvent(CertificateTopic certificateTopic) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", TagTopic.TopicName},
                    {nameof(certificateTopic.CertificateNo), certificateTopic.CertificateNo},
                    {nameof(certificateTopic.CertificateType), certificateTopic.CertificateType.ToString()},
                    {nameof(certificateTopic.CertificateStatus), certificateTopic.CertificateStatus.ToString()},
                    {nameof(certificateTopic.Plant), NormalizePlant(certificateTopic.Plant)},
                    {nameof(certificateTopic.ProjectName), NormalizeProjectName(certificateTopic.ProjectName)}
                });

        private string NormalizePlant(string plant) => plant[4..];

        private string NormalizeProjectName(string projectName) => projectName.Replace('$', '_');
    }
}
