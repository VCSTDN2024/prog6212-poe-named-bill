using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CMCS.Controllers;
using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace CMCS.Tests
{
    // Simple ISession implementation for tests
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();
        public IEnumerable<string> Keys => _store.Keys;
        public string Id { get; } = Guid.NewGuid().ToString();
        public bool IsAvailable { get; } = true;
        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value);
    }

    public static class SessionExtensionsForTests
    {
        public static void SetString(this ISession session, string key, string value)
        {
            session.Set(key, System.Text.Encoding.UTF8.GetBytes(value));
        }

        public static string GetString(this ISession session, string key)
        {
            if (session.TryGetValue(key, out var b)) return System.Text.Encoding.UTF8.GetString(b);
            return null;
        }
    }

    public class ControllerAuthorizationTests
    {
        [Fact]
        public void Create_Forbidden_ForNonLecturer()
        {
            var repo = new InMemoryClaimRepository();
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            var automation = new ClaimAutomationService();
            var controller = new ClaimsController(repo, envMock.Object, automation);

            var context = new DefaultHttpContext();
            var session = new TestSession();
            session.SetString("username", "bob");
            session.SetString("role", "Coordinator");
            context.Session = session;
            controller.ControllerContext = new ControllerContext { HttpContext = context };

            var result = controller.Create();
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void Verify_Allowed_ForCoordinator()
        {
            var repo = new InMemoryClaimRepository();
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            var automation = new ClaimAutomationService();
            var controller = new ClaimsController(repo, envMock.Object, automation);

            var context = new DefaultHttpContext();
            var session = new TestSession();
            session.SetString("username", "bob");
            session.SetString("role", "Coordinator");
            context.Session = session;
            controller.ControllerContext = new ControllerContext { HttpContext = context };

            var claim = repo.Add(new Claim { HoursWorked = 1, HourlyRate = 10 });
            var post = controller.Verify(claim.Id);
            Assert.IsType<RedirectToActionResult>(post);
            var fetched = repo.Get(claim.Id);
            Assert.Equal(ClaimStatus.Verified, fetched.Status);
        }
    }
}
