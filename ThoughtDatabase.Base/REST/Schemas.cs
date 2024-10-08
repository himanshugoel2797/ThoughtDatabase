﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThoughtDatabase.REST
{
	public record AuthRequired(string Token);

	public record UserRequest(string Username, string Password, bool RememberMe);

	public record UserData(string Username, bool IsAdmin, DateTime DateOfRegistration);

	public record AdminStatus(string Token, bool IsAdmin, string Username) : AuthRequired(Token);

	public record DeleteUserRequest(string Token, string Username) : AuthRequired(Token);

	public record RegisterAppRequest(string Token, string Name) : AuthRequired(Token);

	public record ThoughtDatasetListRequest(string Token, int Skip = 0, int? Count = null) : AuthRequired(Token);

	public record ThoughtDatasetCreateInfo(string Token, string Name, string? Description) : AuthRequired(Token);
}
