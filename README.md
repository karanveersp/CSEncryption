# CS Encryption

A dotnet CLI app to encrypt and decrypt the contents of a plaintext format file.

Use it to encrypt any text data you want to share with someone in a secure way. The other person can decrypt the cipher text if they input the right key.

This project can be run using the `dotnet` cli. The user provides they key, and a path
to a plaintext file. The program will encrypt/decrypt depending on the mode, and write
a new file in the same directory as the path provided. 

The new file will have `_cipher` appended to the name if encrypting, and `_plain` appended if decrypting.

The original file is not overwritten.

## Encryption

Provide the `-e` flag to enable encryption mode.

```shell
> dotnet run -key mySecretKey -path path/to/my/note.txt -e
Encrypt mode active
Wrote encrypted file to: path/to/my/note_cipher.txt

The `note_cipher.txt` file can now be shared with anyone. Only the person posessing the key can decrypt the cipher.
```

## Decryption

When the `-e` flag is not provided, the default mode is decryption.

```shell
> dotnet run -key mySecretKey -path path/to/my/note_cipher.txt
Decrypt mode active
Wrote decrypted file to: path/to/my/note_plain.txt
```

## Running and Building the cli app on your system

You can use this repo to build the application from source.

First, clone this repo and make sure to have `dotnet` cli tool installed with version `net5.0`.

Then you can run the following commands to generate a sharable, standalone executable for your platform
that doesn't require the user to have the dotnet runtime on their system.

The `--runtime` flag specifies the target to build for. More information at https://docs.microsoft.com/en-us/dotnet/core/rid-catalog.
The `\p:PublishTrimmed=true` parameter allows trimming the artifact from all unneeded .net libraries.
The `--self-contained` flag produces an artifact that doesn't require the dotnet runtime to be pre-installed.

For sharing with Linux users
```shell
❯ dotnet publish --runtime linux-x64 /p:PublishTrimmed=true --self-contained true
```

For sharing with Windows users
```shell
❯ dotnet publish --runtime win-x64 /p:PublishTrimmed=true --self-contained true
```