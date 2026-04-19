# AGENTS.md — ERPDesktop

Leia **sempre** antes de alterar código deste projeto:

1. **Regra visual / produto (obrigatória)**  
   `ERPDesktop/.cursor/rules/erp-referencia-visual.mdc`  
   — Shell MDI, menus modais 600×300, pesquisas amplas, cadastros em grade, tema WinForms nativo.

2. **Stack**  
   WinForms .NET 8, SQLite, camadas `App` / `Application` / `Domain` / `Infrastructure`.

3. **Build**  
   `dotnet build ERPDesktop.sln -c Release` (a partir da pasta `ERPDesktop`).  
   Testes: `dotnet test ERPDesktop.sln -c Release`. CI: `.github/workflows/dotnet.yml` (runner `windows-latest` por causa do WinForms). Logs da app: `%LocalAppData%\ERPDesktop\Logs\` (Serilog, ficheiro diário).

4. **Layout**  
   Utilitários em `src/ERPDesktop.App/Ui/` (`ErpTheme`, `ErpChrome`, `ErpGridStyle`, `ErpFormLayout`, `ToolbarSpriteHelper`, etc.); preferir reutilizar em novas telas. Ícones da barra: folha 4×6 em `Resources/erp_toolbar_sprite.png` + `Shared/Ui/ToolbarSpriteSheet.cs`.

Não existe outro ficheiro “Project Rule” no repositório além da regra Cursor acima.
