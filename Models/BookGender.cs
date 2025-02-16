﻿namespace PatsyLibrary.Models;

public class BookGender
{
    public BookGender(string name)
    {
        SetName(name);
    }

    public short BookGenderId { get; private set; }
    public string Name { get; private set; }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do gênero do livro não pode ser vazio ou nulo.");

        Name = name;
    }
}
