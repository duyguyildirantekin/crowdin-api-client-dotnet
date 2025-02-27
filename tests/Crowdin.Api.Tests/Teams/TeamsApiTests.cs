﻿
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Crowdin.Api.Core;
using Crowdin.Api.Teams;
using Crowdin.Api.Tests.Core;
using Crowdin.Api.Users;

using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Crowdin.Api.Tests.Teams
{
    public class TeamsApiTests
    {
        private static readonly JsonSerializerSettings DefaultSettings = TestUtils.CreateJsonSerializerOptions();

        [Fact]
        public async Task AddTeamToProject()
        {
            const int projectId = 1;

            var request = new AddTeamToProjectRequest
            {
                TeamId = 1,
                AccessToAllWorkflowSteps = false,
                ManagerAccess = true,
                Permissions = new Dictionary<string, LanguagePermission>
                {
                    {
                        "it",
                        new LanguagePermission
                        {
                            WorkflowStepIds = new[] { 313 }
                        }
                    },
                    {
                        "de",
                        new LanguagePermission
                        {
                            WorkflowStepIds = "all"
                        }
                    }
                }
            };

            string actualRequestJson = JsonConvert.SerializeObject(request, DefaultSettings);
            string expectedRequestJson = TestUtils.CompactJson(Core.Resources.Teams.AddTeamToProject_Request);
            Assert.Equal(expectedRequestJson, actualRequestJson);

            Mock<ICrowdinApiClient> mockClient = TestUtils.CreateMockClientWithDefaultParser();

            var url = $"/projects/{projectId}/teams";

            mockClient
                .Setup(client => client.SendPostRequest(url, request, null))
                .ReturnsAsync(new CrowdinApiResult
                {
                    StatusCode = HttpStatusCode.Created,
                    JsonObject = JObject.Parse(Core.Resources.Teams.AddTeamToProject_Response)
                });

            var executor = new TeamsApiExecutor(mockClient.Object);
            ProjectTeamResources response = await executor.AddTeamToProject(projectId, request);
            
            Assert.NotNull(response.Skipped?.Permissions);
            Assert.Contains("it", response.Skipped!.Permissions);
            
            int[]? workflowStepIds = JArray.FromObject(response.Skipped!.Permissions["it"].WorkflowStepIds).ToObject<int[]>();
            Assert.NotNull(workflowStepIds);
            Assert.Equal(313, workflowStepIds![0]);
        }
    }
}