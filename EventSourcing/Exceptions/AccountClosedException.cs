namespace EventSourcing.Exceptions;

public class AccountClosedException(string message) : InvalidOperationException(message);