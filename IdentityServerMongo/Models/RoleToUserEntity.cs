﻿using System;

namespace IdentityServerMongo.Models
{
    public class RoleToUserEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}