﻿namespace API;

public class LikeParams : PaginationParams
{
    public int UserId { get; set; }
    public string Predicate { get; set; }
}
