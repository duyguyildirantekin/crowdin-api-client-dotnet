﻿
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Crowdin.Api.Core;
using JetBrains.Annotations;

#nullable enable

namespace Crowdin.Api.ProjectsGroups
{
    public class ProjectsGroupsApiExecutor
    {
        private const string BaseGroupsSubUrl = "/groups";
        private const string BaseProjectsSubUrl = "/projects";
        private readonly ICrowdinApiClient _apiClient;
        private readonly IJsonParser _jsonParser;

        public ProjectsGroupsApiExecutor(ICrowdinApiClient apiClient)
        {
            _apiClient = apiClient;
            _jsonParser = apiClient.DefaultJsonParser;
        }

        public ProjectsGroupsApiExecutor(ICrowdinApiClient apiClient, IJsonParser jsonParser)
        {
            _apiClient = apiClient;
            _jsonParser = jsonParser;
        }

        #region Groups

        /// <summary>
        /// List groups. Documentation:
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.groups.getMany">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task<ResponseList<Group>> ListGroups(int? parentId, int limit = 25, int offset = 0)
        {
            IDictionary<string, string> queryParams = Utils.CreateQueryParamsFromPaging(limit, offset);
            queryParams.AddParamIfPresent("parentId", parentId);

            CrowdinApiResult result = await _apiClient.SendGetRequest(BaseGroupsSubUrl, queryParams);
            return _jsonParser.ParseResponseList<Group>(result.JsonObject);
        }

        /// <summary>
        /// Add group. Documentation:
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.groups.post">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task<Group> AddGroup(AddGroupRequest request)
        {
            CrowdinApiResult result = await _apiClient.SendPostRequest(BaseGroupsSubUrl, request);
            return _jsonParser.ParseResponseObject<Group>(result.JsonObject);
        }

        /// <summary>
        /// Get group. Documentation:
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.groups.get">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task<Group> GetGroup(int groupId)
        {
            string url = FormUrl_GroupId(groupId);
            CrowdinApiResult result = await _apiClient.SendGetRequest(url);
            return _jsonParser.ParseResponseObject<Group>(result.JsonObject);
        }

        /// <summary>
        /// Delete group. Documentation:
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.groups.delete">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task DeleteGroup(int groupId)
        {
            string url = FormUrl_GroupId(groupId);
            HttpStatusCode statusCode = await _apiClient.SendDeleteRequest(url);
            Utils.ThrowIfStatusNot204(statusCode, $"Group {groupId} removal failed");
        }

        /// <summary>
        /// Edit group. Documentation:
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.groups.patch">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task<Group> EditGroup(int groupId, IEnumerable<GroupPatch> patches)
        {
            string url = FormUrl_GroupId(groupId);
            CrowdinApiResult result = await _apiClient.SendPatchRequest(url, patches);
            return _jsonParser.ParseResponseObject<Group>(result.JsonObject);
        }

        #region Helper methods

        private static string FormUrl_GroupId(int groupId) => $"{BaseGroupsSubUrl}/{groupId}";

        #endregion

        #endregion

        #region Projects

        /// <summary>
        /// List projects. Documentation:
        /// <a href="https://support.crowdin.com/api/v2/#operation/api.projects.getMany">Crowdin API</a>
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.projects.getMany">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task<ResponseList<TProject>> ListProjects<TProject>(
            int? userId = null, int? groupId = null,
            bool hasManagerAccess = false,
            int limit = 25, int offset = 0)
                where TProject : ProjectBase // Project, EnterpriseProject
        {
            IDictionary<string, string> queryParams = Utils.CreateQueryParamsFromPaging(limit, offset);
            queryParams.AddParamIfPresent("userId", userId);
            queryParams.AddParamIfPresent("groupId", groupId);
            queryParams.Add("hasManagerAccess", hasManagerAccess ? "1" : "0");

            CrowdinApiResult result = await _apiClient.SendGetRequest(BaseProjectsSubUrl, queryParams);
            
            return _jsonParser.ParseResponseList<TProject>(result.JsonObject);
        }

        /// <summary>
        /// Add project. Documentation:
        /// <a href="https://support.crowdin.com/api/v2/#operation/api.projects.post">Crowdin API</a>
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.projects.post">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task<T> AddProject<T>(AddProjectRequest request)
            where T : ProjectBase // Project { ProjectSettings }, EnterpriseProject
        {
            CrowdinApiResult result = await _apiClient.SendPostRequest(BaseProjectsSubUrl, request);
            return _jsonParser.ParseResponseObject<T>(result.JsonObject);
        }

        /// <summary>
        /// Get project. Documentation:
        /// <a href="https://support.crowdin.com/api/v2/#operation/api.projects.get">Crowdin API</a>
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.projects.get">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task<T> GetProject<T>(int projectId)
            where T : ProjectBase
        {
            CrowdinApiResult result = await _apiClient.SendGetRequest(FormUrl_ProjectId(projectId));
            return _jsonParser.ParseResponseObject<T>(result.JsonObject);
        }

        /// <summary>
        /// Delete project. Documentation:
        /// <a href="https://support.crowdin.com/api/v2/#operation/api.projects.delete">Crowdin API</a>
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.projects.delete">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task DeleteProject(int projectId)
        {
            HttpStatusCode statusCode = await _apiClient.SendDeleteRequest(FormUrl_ProjectId(projectId));
            Utils.ThrowIfStatusNot204(statusCode, $"Project {projectId} removal failed");
        }

        /// <summary>
        /// Edit project. Documentation:
        /// <a href="https://support.crowdin.com/api/v2/#operation/api.projects.patch">Crowdin API</a>
        /// <a href="https://support.crowdin.com/enterprise/api/#operation/api.projects.patch">Crowdin Enterprise API</a>
        /// </summary>
        [PublicAPI]
        public async Task<T> EditProject<T>(int projectId, IEnumerable<ProjectPatch> patches)
            where T : ProjectBase
        {
            CrowdinApiResult result = await _apiClient.SendPatchRequest(FormUrl_ProjectId(projectId), patches);
            return _jsonParser.ParseResponseObject<T>(result.JsonObject);
        }

        #region Helper methods

        private static string FormUrl_ProjectId(int projectId) => $"{BaseProjectsSubUrl}/{projectId.ToString()}";

        #endregion

        #endregion
    }
}