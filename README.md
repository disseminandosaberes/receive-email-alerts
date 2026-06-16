# InfraStellar

Projeto ASP.NET Core com Razor Pages seguindo arquitetura em camadas (Clean Architecture).

## Estrutura

```
src/
├── Domain/          # Entidades, interfaces e regras de negócio
├── Application/     # Casos de uso e serviços de aplicação
├── Infrastructure/  # Persistência, serviços externos e implementações
└── Web/             # Razor Pages — camada de apresentação
```

## Tecnologias

- [.NET 8](https://dotnet.microsoft.com/)
- ASP.NET Core Razor Pages
- Clean Architecture

## Como rodar

```bash
cd src/Web
dotnet run
```
