using Microsoft.EntityFrameworkCore;
using MyBooks.Data;
using MyBooks.Data.Models;
using MyBooks.Data.Services;
using MyBooks.Data.Views;

namespace MyBooksTests;

public class PublisherServiceTest
{
    private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "mybookstest")
        .Options;

    AppDbContext context;
    PublisherService publisherService;

    [OneTimeSetUp]
    public void Setup()
    {
        context = new AppDbContext(dbContextOptions);
        context.Database.EnsureCreated();

        SeedDatabase();

        publisherService = new PublisherService(context);
    }

    [Test, Order(1)]
    public void GetPublishers_WithoutParameter()
    {
        var result = publisherService.GetPublishers("", "", "", 1, 10);

        Assert.That(result.Items.Count, Is.EqualTo(3));
    }

    [Test, Order(2)]
    public void GetPublishers_WithPageNumber_WithPageSize()
    {
        var result = publisherService.GetPublishers("", "", "", 2, 2);

        Assert.That(result.Items.Count, Is.EqualTo(1));
    }

    [Test, Order(3)]
    public void GetPublishers_WithSearch()
    {
        var result = publisherService.GetPublishers("Name", "", "3", 1, 10);

        Assert.That(result.Items.FirstOrDefault().Name, Is.EqualTo("Publisher 3"));
    }

    [Test, Order(4)]
    public void GetPublishers_WithSort()
    {
        var result = publisherService.GetPublishers("Name", "desc", "", 1, 10);

        Assert.That(result.Items.FirstOrDefault().Name, Is.EqualTo("Publisher 3"));
    }

    [Test, Order(5)]
    public void GetPublisherById()
    {
        var result = publisherService.GetPublisherById(1);

        Assert.That(result.Name, Is.EqualTo("Publisher 1"));
        Assert.That(result.BookAuthors.Count, Is.EqualTo(2));
    }

    [Test, Order(6)]
    public void GetPublisherById_NotFound()
    {
        Assert.That(() => publisherService.GetPublisherById(4), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Publisher with id : 4 does not exists"));
    }

    [Test, Order(7)]
    public void AddPublisher()
    {
        var newPublisher = new PublisherViews()
        {
            Name = "Aditya",
        };

        var result = publisherService.AddPublisher(newPublisher);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Aditya"));
    }

    [Test, Order(8)]
    public void DeletePublisher_NotFound()
    {
        Assert.That(() => publisherService.DeletePublisher(44), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Publisher with id : 44 does not exists"));
    }

    [Test, Order(9)]
    public void DeletePublisher()
    {
        publisherService.DeletePublisher(4);
        Assert.That(() => publisherService.GetPublisherById(4), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Publisher with id : 4 does not exists"));
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
