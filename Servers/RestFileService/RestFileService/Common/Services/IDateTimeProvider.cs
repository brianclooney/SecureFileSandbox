﻿namespace RestFileService.Common.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
