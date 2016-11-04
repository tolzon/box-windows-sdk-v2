﻿using Box.V2.Config;
using Box.V2.Managers;
using Box.V2.Models;
using Box.V2.Models.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Box.V2.Test
{
    [TestClass]
    public class BoxTasksManagerTest : BoxResourceManagerTest
    {
        protected BoxTasksManager _tasksManager;

        public BoxTasksManagerTest()
        {
            _tasksManager = new BoxTasksManager(_config.Object, _service, _converter, _authRepository);
        }

        [TestMethod]
        public async Task CreateTaskAssignment_ValidResponse()
        {
            /*** Arrange ***/
            string responseString = @"{
                                        ""type"": ""task_assignment"",
                                        ""id"": ""2698512"",
                                        ""item"": {
                                            ""type"": ""file"",
                                            ""id"": ""8018809384"",
                                            ""sequence_id"": ""0"",
                                            ""etag"": ""0"",
                                            ""sha1"": ""7840095ee096ee8297676a138d4e316eabb3ec96"",
                                            ""name"": ""scrumworksToTrello.js""
                                        },
                                        ""assigned_to"": {
                                            ""type"": ""user"",
                                            ""id"": ""1992432"",
                                            ""name"": ""rhaegar@box.com"",
                                            ""login"": ""rhaegar@box.com""
                                        },
                                        ""message"": null,
                                        ""completed_at"": null,
                                        ""assigned_at"": ""2013-05-10T11:43:41-07:00"",
                                        ""reminded_at"": null,
                                        ""resolution_state"": ""incomplete"",
                                        ""assigned_by"": {
                                            ""type"": ""user"",
                                            ""id"": ""11993747"",
                                            ""name"": ""sean"",
                                            ""login"": ""sean@box.com""
                                        }
                                    }";
            IBoxRequest boxRequest = null;
            Uri taskAssignmentsUri = new Uri(Constants.TaskAssignmentsEndpointString);
            _config.SetupGet(x => x.TaskAssignmentsEndpointUri).Returns(taskAssignmentsUri);
            _handler.Setup(h => h.ExecuteAsync<BoxTaskAssignment>(It.IsAny<IBoxRequest>()))
                .Returns(Task.FromResult<IBoxResponse<BoxTaskAssignment>>(new BoxResponse<BoxTaskAssignment>()
                {
                    Status = ResponseStatus.Success,
                    ContentString = responseString
                }))
                .Callback<IBoxRequest>(r => boxRequest = r);

            /*** Act ***/
            BoxTaskAssignmentRequest taskAssignmentRequest = new BoxTaskAssignmentRequest()
            {
                Task = new BoxTaskRequest()
                {
                    Id = "1992432"
                },
                AssignTo = new BoxAssignmentRequest()
                {
                    Id = "1992432"
                }
            };
            BoxTaskAssignment result = await _tasksManager.CreateTaskAssignmentAsync(taskAssignmentRequest);

            /*** Assert ***/
            //Request check
            Assert.IsNotNull(boxRequest);
            Assert.AreEqual(RequestMethod.Post, boxRequest.Method);
            Assert.AreEqual(taskAssignmentsUri, boxRequest.AbsoluteUri.AbsoluteUri);
            BoxTaskAssignmentRequest payload = JsonConvert.DeserializeObject<BoxTaskAssignmentRequest>(boxRequest.Payload);
            Assert.AreEqual(taskAssignmentRequest.Task.Id, payload.Task.Id);
            Assert.AreEqual(taskAssignmentRequest.Task.Type, payload.Task.Type);
            Assert.AreEqual(taskAssignmentRequest.AssignTo.Id, payload.AssignTo.Id);
            Assert.AreEqual(taskAssignmentRequest.AssignTo.Login, payload.AssignTo.Login);

            //Response check
            Assert.AreEqual("2698512", result.Id);
            Assert.AreEqual("task_assignment", result.Type);
            Assert.AreEqual("8018809384", result.Item.Id);
            Assert.AreEqual("file", result.Item.Type);
            Assert.AreEqual("0", result.Item.ETag);
            Assert.AreEqual(ResolutionStateType.incomplete, result.ResolutionState);
            Assert.AreEqual("sean@box.com", result.AssignedBy.Login);
            Assert.AreEqual("11993747", result.AssignedBy.Id);
            Assert.AreEqual("rhaegar@box.com", result.AssignedTo.Login);
            Assert.AreEqual("1992432", result.AssignedTo.Id);
        }

        [TestMethod]
        public async Task UpdateTaskAssignment_ValidResponse()
        {
            /*** Arrange ***/
            string responseString = @"{
                                            ""type"": ""task_assignment"",
                                            ""id"": ""2698512"",
                                            ""item"": {
                                                ""type"": ""file"",
                                                ""id"": ""8018809384"",
                                                ""sequence_id"": ""0"",
                                                ""etag"": ""0"",
                                                ""sha1"": ""7840095ee096ee8297676a138d4e316eabb3ec96"",
                                                ""name"": ""scrumworksToTrello.js""
                                            },
                                            ""assigned_to"": {
                                                ""type"": ""user"",
                                                ""id"": ""1992432"",
                                                ""name"": ""rhaegar@box.com"",
                                                ""login"": ""rhaegar@box.com""
                                            },
                                            ""message"": ""hello!!!"",
                                            ""completed_at"": null,
                                            ""assigned_at"": ""2013-05-10T11:43:41-07:00"",
                                            ""reminded_at"": null,
                                            ""resolution_state"": ""incomplete"",
                                            ""assigned_by"": {
                                                ""type"": ""user"",
                                                ""id"": ""11993747"",
                                                ""name"": ""sean"",
                                                ""login"": ""sean@box.com""
                                            }
                                        }";
            IBoxRequest boxRequest = null;
            Uri taskAssignmentsUri = new Uri(Constants.TaskAssignmentsEndpointString);
            _config.SetupGet(x => x.TaskAssignmentsEndpointUri).Returns(taskAssignmentsUri);
            _handler.Setup(h => h.ExecuteAsync<BoxTaskAssignment>(It.IsAny<IBoxRequest>()))
                .Returns(Task.FromResult<IBoxResponse<BoxTaskAssignment>>(new BoxResponse<BoxTaskAssignment>()
                {
                    Status = ResponseStatus.Success,
                    ContentString = responseString
                }))
                .Callback<IBoxRequest>(r => boxRequest = r);

            /*** Act ***/
            BoxTaskAssignmentUpdateRequest taskAssignmentUpdateRequest = new BoxTaskAssignmentUpdateRequest()
            {
                Id = "2698512",
                Message = "hello!!!"
            };
            BoxTaskAssignment result = await _tasksManager.UpdateTaskAssignmentAsync(taskAssignmentUpdateRequest);

            /*** Assert ***/
            //Request check
            Assert.IsNotNull(boxRequest);
            Assert.AreEqual(RequestMethod.Put, boxRequest.Method);
            Assert.AreEqual(taskAssignmentsUri + taskAssignmentUpdateRequest.Id, boxRequest.AbsoluteUri.AbsoluteUri);
            BoxTaskAssignmentUpdateRequest payload = JsonConvert.DeserializeObject<BoxTaskAssignmentUpdateRequest>(boxRequest.Payload);
            Assert.AreEqual(taskAssignmentUpdateRequest.Id, payload.Id);
            Assert.AreEqual(taskAssignmentUpdateRequest.Message, payload.Message);

            //Response check
            Assert.AreEqual("2698512", result.Id);
            Assert.AreEqual("task_assignment", result.Type);
            Assert.AreEqual("hello!!!", result.Message);
            Assert.AreEqual("8018809384", result.Item.Id);
            Assert.AreEqual("file", result.Item.Type);
            Assert.AreEqual("0", result.Item.ETag);
            Assert.AreEqual(ResolutionStateType.incomplete, result.ResolutionState);
            Assert.AreEqual("sean@box.com", result.AssignedBy.Login);
            Assert.AreEqual("11993747", result.AssignedBy.Id);
            Assert.AreEqual("rhaegar@box.com", result.AssignedTo.Login);
            Assert.AreEqual("1992432", result.AssignedTo.Id);
        }

        [TestMethod]
        public async Task GetTaskAssignment_ValidResponse()
        {
            /*** Arrange ***/
            string responseString = @"{
                                        ""type"": ""task_assignment"",
                                        ""id"": ""2698512"",
                                        ""item"": {
                                            ""type"": ""file"",
                                            ""id"": ""8018809384"",
                                            ""sequence_id"": ""0"",
                                            ""etag"": ""0"",
                                            ""sha1"": ""7840095ee096ee8297676a138d4e316eabb3ec96"",
                                            ""name"": ""scrumworksToTrello.js""
                                        },
                                        ""assigned_to"": {
                                            ""type"": ""user"",
                                            ""id"": ""1992432"",
                                            ""name"": ""rhaegar@box.com"",
                                            ""login"": ""rhaegar@box.com""
                                        },
                                        ""message"": null,
                                        ""completed_at"": null,
                                        ""assigned_at"": ""2013-05-10T11:43:41-07:00"",
                                        ""reminded_at"": null,
                                        ""resolution_state"": ""incomplete"",
                                        ""assigned_by"": {
                                            ""type"": ""user"",
                                            ""id"": ""11993747"",
                                            ""name"": ""sean"",
                                            ""login"": ""sean@box.com""
                                        }
                                    }";
            IBoxRequest boxRequest = null;
            Uri taskAssignmentsUri = new Uri(Constants.TaskAssignmentsEndpointString);
            _config.SetupGet(x => x.TaskAssignmentsEndpointUri).Returns(taskAssignmentsUri);
            _handler.Setup(h => h.ExecuteAsync<BoxTaskAssignment>(It.IsAny<IBoxRequest>()))
                .Returns(Task.FromResult<IBoxResponse<BoxTaskAssignment>>(new BoxResponse<BoxTaskAssignment>()
                {
                    Status = ResponseStatus.Success,
                    ContentString = responseString
                }))
                .Callback<IBoxRequest>(r => boxRequest = r);

            /*** Act ***/

            BoxTaskAssignment result = await _tasksManager.GetTaskAssignmentAsync("2698512");

            /*** Assert ***/
            //Request check
            Assert.IsNotNull(boxRequest);
            Assert.AreEqual(RequestMethod.Get, boxRequest.Method);
            Assert.AreEqual(taskAssignmentsUri + "2698512", boxRequest.AbsoluteUri.AbsoluteUri);

            //Response check
            Assert.AreEqual("2698512", result.Id);
            Assert.AreEqual("task_assignment", result.Type);
            Assert.AreEqual("8018809384", result.Item.Id);
            Assert.AreEqual("file", result.Item.Type);
            Assert.AreEqual("0", result.Item.ETag);
            Assert.AreEqual(ResolutionStateType.incomplete, result.ResolutionState);
            Assert.AreEqual("sean@box.com", result.AssignedBy.Login);
            Assert.AreEqual("11993747", result.AssignedBy.Id);
            Assert.AreEqual("rhaegar@box.com", result.AssignedTo.Login);
            Assert.AreEqual("1992432", result.AssignedTo.Id);
        }

        [TestMethod]
        public async Task DeleteTaskAssignment_TaskAssignmentDeleted()
        {
            /*** Arrange ***/
            string responseString = "";
            IBoxRequest boxRequest = null;
            Uri taskAssignmentsUri = new Uri(Constants.TaskAssignmentsEndpointString);
            _config.SetupGet(x => x.TaskAssignmentsEndpointUri).Returns(taskAssignmentsUri);
            _handler.Setup(h => h.ExecuteAsync<BoxTaskAssignment>(It.IsAny<IBoxRequest>()))
                .Returns(Task.FromResult<IBoxResponse<BoxTaskAssignment>>(new BoxResponse<BoxTaskAssignment>()
                {
                    Status = ResponseStatus.Success,
                    ContentString = responseString
                }))
                .Callback<IBoxRequest>(r => boxRequest = r);

            /*** Act ***/
            bool result = await _tasksManager.DeleteTaskAssignmentAsync("2698512");

            /*** Assert ***/
            //Request check
            Assert.IsNotNull(boxRequest);
            Assert.AreEqual(RequestMethod.Delete, boxRequest.Method);
            Assert.AreEqual(taskAssignmentsUri + "2698512", boxRequest.AbsoluteUri.AbsoluteUri);
          
            //Response check
            Assert.AreEqual(true, result);


        }
    }
}
