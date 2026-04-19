# ERP Desktop

ERP de **loja de calçados** em **WinForms (.NET 8)** com **SQLite** local. Camadas: `App` → `Application` → `Domain` → `Infrastructure`.

## Requisitos

- **Windows** (interface WinForms)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (recomendado: versão **estável** LTS, não preview)

## Compilar e executar

Na raiz do repositório (pasta que contém `ERPDesktop.sln`):

```powershell
dotnet build ERPDesktop.sln -c Release
dotnet run --project src\ERPDesktop.App\ERPDesktop.App.csproj -c Release
```

## Testes e CI

```powershell
dotnet test ERPDesktop.sln -c Release
```

Integração contínua: [`.github/workflows/dotnet.yml`](.github/workflows/dotnet.yml) (runner **windows-latest**).

## Dados

A base **SQLite** é criada automaticamente em:

`%LocalAppData%\ERPDesktop\Data\erpdesktop.db`

## Logs

Ficheiros **Serilog** (rotação diária, 14 dias): `%LocalAppData%\ERPDesktop\Logs\erp-YYYYMMDD.log`

## Ícones da barra principal

Folha **4×6** em [`src/ERPDesktop.App/Resources/erp_toolbar_sprite.png`](src/ERPDesktop.App/Resources/erp_toolbar_sprite.png): recorte e escala em `ToolbarSpriteSheet` / `ToolbarSpriteHelper` (DPI + evento `DpiChanged`). Para mudar o mapeamento botão → célula, edita `MainToolbarCells` em `ToolbarSpriteHelper.cs`.

## Documentação interna

- [`AGENTS.md`](AGENTS.md) — convenções para agentes e IA ao alterar o projeto
- [`COMO_RODAR.txt`](COMO_RODAR.txt) — passos resumidos

## Licença

Ver [LICENSE](LICENSE).
