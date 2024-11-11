namespace EventSourcing.Exceptions;

public class AccountDisabledException(string message) : InvalidOperationException(message);