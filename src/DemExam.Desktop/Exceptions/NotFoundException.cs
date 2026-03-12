namespace DemExam.Desktop.Exceptions;

public class NotFoundException(string? message = null) : Exception(message)
{
}