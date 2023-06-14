# Simple IPC
A simple-to-use IPC library using named pipes and MessagePack.

## Links
NuGet: https://www.nuget.org/packages/immense.SimpleIpc/  

## Quick Start

With Dependency Injection:
```
var serviceCollection = new ServiceCollection();
serviceCollection.AddSimpleIpc();
var provider = serviceCollection.BuildServiceProvider();

var router = provider.GetRequiredService<IIpcRouter>();
var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
```

Without Dependency Injection:
```
var router = SimpleIpc.Router.Default;
var connectionFactory = SimpleIpc.ConnectionFactory.Default;
```

Then:
```
var pipeName = Guid.NewGuid().ToString();

// The Router can be used to retrieve server instances elsewhere in code.
var server = await router.CreateServer(_pipeName);
var client = await connectionFactory.CreateClient(".", _pipeName);

// Server must wait for connection.
_ = server.WaitForConnection(_cts.Token);

// Connect will return true if successful.
var result = await client.Connect(1000);

// Register callbacks that will handle message types.  When calling Send from the other end of the pipe, this end will handle it with the supplied callback.  The On method will return a token that can be used to unregister callbacks.
var callbackToken = client.On((Ping ping) =>
{
    Console.WriteLine("Received ping from server.");
    client.Send(new Pong("Pong from client"));
});

// Handlers can return values, which can be retrieved from the other end via Invoke (instead of Send).
server.On((Ping ping) =>
{
    Console.WriteLine("Received ping from client.");
    return new Pong("Pong from server");
});

server.On((Pong pong) =>
{
    Console.WriteLine("Received pong from client.");
});

client.BeginRead(_cts.Token);
server.BeginRead(_cts.Token);

// Ping is sent asyncronously and will be handled on the server.
await server.Send(new Ping());

// Pong value is retrieved directly from server.
var pong = await client.Invoke<Ping, Pong>(new Ping());

// Remove a specific callback.
client.Off<Ping>(callbackToken);

// Remove all callbacks of a given type.
server.Off<Pong>();
```