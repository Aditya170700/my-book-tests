using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBooks.Controllers;
using MyBooks.Data;
using MyBooks.Data.Models;
using MyBooks.Data.Services;
using MyBooks.Data.Views;

namespace MyBooksTests;

public class PublisherControllerTest
{
    private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "mybookstestctrl")
        .Options;

    AppDbContext context;
    PublisherService publisherService;
    PublisherController publisherController;

    [OneTimeSetUp]
    public void Setup()
    {
        context = new AppDbContext(dbContextOptions);
        context.Database.EnsureCreated();

        SeedDatabase();

        publisherService = new PublisherService(context);
        publisherController = new PublisherController(publisherService);
    }

    [Test, Order(1)]
    public void HttpGet_GetPublishers()
    {
        IActionResult actionResult = publisherController.GetPublishers("Name", "asc", "", 1, 1);

        Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

        var actionResultData = (actionResult as OkObjectResult).Value as PaginatedViews<Publisher>;

        Assert.That(actionResultData.Items.First().Name, Is.EqualTo("Publisher 1"));
        Assert.That(actionResultData.Items.First().Id, Is.EqualTo(1));
        Assert.That(actionResultData.Items.Count, Is.EqualTo(1));

        IActionResult actionResult2 = publisherController.GetPublishers("Name", "asc", "", 2, 1);

        Assert.That(actionResult2, Is.TypeOf<OkObjectResult>());

        var actionResultData2 = (actionResult2 as OkObjectResult).Value as PaginatedViews<Publisher>;

        Assert.That(actionResultData2.Items.First().Name, Is.EqualTo("Publisher 2"));
        Assert.That(actionResultData2.Items.First().Id, Is.EqualTo(2));
        Assert.That(actionResultData2.Items.Count, Is.EqualTo(1));
    }

    [Test, Order(2)]
    public void HttpGet_GetPublisher()
    {
        IActionResult actionResult = publisherController.GetPublisher(1);

        Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

        var actionResultData = (actionResult as OkObjectResult).Value as PublisherBookAuthorsViews;

        Assert.That(actionResultData.Name, Is.EqualTo("Publisher 1"));
        Assert.That(actionResultData.BookAuthors.Count, Is.EqualTo(2));
    }

    [Test, Order(3)]
    public void HttpGet_GetPublisherNotFound()
    {
        Assert.That(() => publisherController.GetPublisher(4), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Publisher with id : 4 does not exists"));
    }

    [Test, Order(4)]
    public void HttpPost_AddPublisher()
    {
        var newPublisher = new PublisherViews()
        {
            Name = "New Publisher",
        };

        IActionResult actionResult = publisherController.AddPublisher(newPublisher);

        Assert.That(actionResult, Is.TypeOf<CreatedResult>());
    }

    [Test, Order(5)]
    public void HttpDelete_DeletePublisher()
    {
        IActionResult actionResult = publisherController.DeletePublisher(1);

        Assert.That(actionResult, Is.TypeOf<OkResult>());
    }

    [Test, Order(6)]
    public void HttpDelete_DeletePublisherNotFound()
    {
        Assert.That(() => publisherController.DeletePublisher(1), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Publisher with id : 1 does not exists"));
    }

    private void SeedDatabase()
    {
        var publishers = new List<Publisher>
        {
            new Publisher() {
                Id = 1,
                Name = "Publisher 1",
            },
            new Publisher() {
                Id = 2,
                Name = "Publisher 2",
            },
            new Publisher() {
                Id = 3,
                Name = "Publisher 3",
            },
        };
        context.Publishers.AddRange(publishers);

        var authors = new List<Author>
        {
            new Author() {
                Id = 1,
                FullName = "Author 1",
            },
            new Author() {
                Id = 2,
                FullName = "Author 2",
            },
        };
        context.Authors.AddRange(authors);

        var books = new List<Book>
        {
            new Book() {
                Id = 1,
                Title = "Book Title 1",
                description = "Book Description 1",
                IsRead = false,
                Genre = "Genre 1",
                CoverUrl = "http://...",
                CreatedAt = DateTime.Now.AddDays(-11),
                PublisherId = 1,
            },
            new Book() {
                Id = 2,
                Title = "Book Title 2",
                description = "Book Description 2",
                IsRead = false,
                Genre = "Genre 1",
                CoverUrl = "http://...",
                CreatedAt = DateTime.Now.AddDays(-11),
                PublisherId = 1,
            },
        };
        context.Books.AddRange(books);

        var bookAuthors = new List<BookAuthor>
        {
            new BookAuthor() {
                Id = 1,
                BookId = 1,
                AuthorId = 1,
            },
            new BookAuthor() {
                Id = 2,
                BookId = 1,
                AuthorId = 2,
            },
            new BookAuthor() {
                Id = 3,
                BookId = 2,
                AuthorId = 2,
            },
        };
        context.BookAuthors.AddRange(bookAuthors);

        context.SaveChanges();
    }

    [OneTimeTearDown]
    public void CleanUp()
    {
        context.Database.EnsureDeleted();
    }
}
