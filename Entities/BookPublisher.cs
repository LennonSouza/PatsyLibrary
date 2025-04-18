﻿namespace PatsyLibrary.Models;

public class BookPublisher
{
    public BookPublisher(string name)
    {
        UpdateName(name);
    }

    public short BookPublisherId { get; private set; }
    public string Name { get; private set; }

    public virtual ICollection<Book> Books { get; set; }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome da editora não pode ser vazio ou nulo.");

        Name = name;
    }

    public BookPublisher() { }

    public void UpdateBookPublisher(string name)
    {
        UpdateName(name);
    }
}