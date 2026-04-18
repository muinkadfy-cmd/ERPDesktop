---
description: Retomar o ERPDesktop — onde paramos e próximo passo
---

Retoma o **ERPDesktop** (.NET 8 + WinForms + SQLite + Dapper) como “continuar de onde parei hoje”.

1. Inspeciona o estado do código: `git status`, `src/ERPDesktop.App/Program.cs`, `Forms/MainForm.cs`, outros forms em `Forms/`, `Ui/`, `Infrastructure/` (migrações, DI).
2. Relembra o padrão de UI em `.cursor/rules/erp-referencia-visual.mdc` (menus módulo 600×300, shell MDI, pesquisas/cadastros no estilo ERP).
3. Responde em **português**, objetivo:
   - **Onde paramos** (implementado + ficheiros-chave).
   - **Pendências** (stubs, telas por ligar, tabelas SQLite).
   - **Próximo passo** (1–3 itens concretos).

Sem refactors desnecessários se não houver pedido explícito.
