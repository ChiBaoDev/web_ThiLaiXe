-- =========================================================
-- 01_full_setup_database_team.sql
-- Team script: reset DB -> security -> schema -> migration -> seed -> verify
-- YEU CAU: BAT SQLCMD Mode trong SSMS truoc khi chay
-- Menu: Query -> SQLCMD Mode
-- =========================================================

:r .\00_reset_database_clean.sql
:r .\00_create_login_and_access.sql
:r .\new_database_moto_lise.sql
:r .\seed_admin.sql
:r .\03_add_rbac_entitlement_and_files.sql
:r .\04_add_cms_and_certificate.sql
:r .\05_seed_roles_permissions_entitlements.sql
:r .\06_verify_new_modules.sql

PRINT N'OK: Team full setup completed (reset + security + schema + migration + seed + verify).';
GO
