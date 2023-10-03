# reaction-roles-bot-cs
 rewrite of [reaction-roles-bot](https://github.com/felix920506/reaction-roles-bot) in C# because [Venson](https://github.com/JPVenson) hates python and I got bullied

## How to use
1. Setup all requirements
2. Provide token in token.txt
3. run the program
4. invite bot to discord server
5. grant bot permissions to manage roles
6. setup reaction roles

## commands 

- /reactionroles add <emoji> <role>

    Adds a reaction role to your previous message in this channel

- /reactionroles remove <message id> <emoji>

    Removes the reaction role from message with the specified emoji

- /reactionroles clear <message id>

    Removes all reaction roles from the specified message

## Limitations / Known Issues

Currently, This bot is intended for single server use only. Using it accross multiple servers can cause some fun stuff if the remove / clear commands are abused.
