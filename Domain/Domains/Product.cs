﻿using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Domains
{
    public class Product
    {
        public Product(Guid id, string description, decimal price)
        {
            Id = id;
            Description = description;
            Price = price;
        }
        
        public Guid Id { get; private set; }
        
        public string Description { get; private set; }

        public decimal Price { get; private set; }
    }
}