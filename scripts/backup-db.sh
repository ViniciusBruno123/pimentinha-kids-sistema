#!/usr/bin/env bash
#
# Backup do banco MySQL do CRBVendas (Pimentinha Kids).
#
# Uso local (backup manual, agora):
#   ./scripts/backup-db.sh
#
# Uso agendado (cron), lendo credenciais de um arquivo protegido:
#   0 3 * * * /caminho/para/scripts/backup-db.sh >> ~/backups/crbvendas/backup.log 2>&1
#
# Credenciais: o script procura um arquivo no formato "my.cnf" em
# CRED_FILE (padrão: ~/.crbvendas-backup.cnf). Crie-o assim, com
# permissão restrita, para a senha nunca aparecer em "ps aux" nem
# no histórico do shell:
#
#   cat > ~/.crbvendas-backup.cnf <<'EOF'
#   [client]
#   user=crbvendas
#   password=SUA_SENHA_AQUI
#   host=localhost
#   port=3306
#   EOF
#   chmod 600 ~/.crbvendas-backup.cnf
#
# Backup remoto (opcional): defina VPS_HOST (ex: usuario@meuservidor.com)
# para copiar cada backup também para o seu VPS via scp, usando a chave
# ~/.ssh/id_ed25519_vps.

set -euo pipefail

DB_NAME="${DB_NAME:-crbvendas}"
CRED_FILE="${CRED_FILE:-$HOME/.crbvendas-backup.cnf}"
BACKUP_DIR="${BACKUP_DIR:-$HOME/backups/crbvendas}"
RETENTION_DIAS="${RETENTION_DIAS:-14}"

if [ ! -f "$CRED_FILE" ]; then
  echo "Arquivo de credenciais não encontrado: $CRED_FILE" >&2
  echo "Veja as instruções no topo deste script para criá-lo." >&2
  exit 1
fi

mkdir -p "$BACKUP_DIR"

TIMESTAMP=$(date +%Y%m%d_%H%M%S)
ARQUIVO="$BACKUP_DIR/${DB_NAME}_${TIMESTAMP}.sql.gz"

mysqldump \
  --defaults-extra-file="$CRED_FILE" \
  --single-transaction \
  --routines \
  --triggers \
  "$DB_NAME" | gzip > "$ARQUIVO"

echo "Backup criado: $ARQUIVO ($(du -h "$ARQUIVO" | cut -f1))"

# Remove backups locais mais antigos que RETENTION_DIAS
find "$BACKUP_DIR" -name "${DB_NAME}_*.sql.gz" -mtime +"$RETENTION_DIAS" -delete

# Cópia opcional para um servidor remoto (proteção contra perda do disco local)
if [ -n "${VPS_HOST:-}" ]; then
  scp -i "${VPS_SSH_KEY:-$HOME/.ssh/id_ed25519_vps}" "$ARQUIVO" \
    "${VPS_HOST}:${VPS_BACKUP_DIR:-backups/crbvendas/}"
  echo "Cópia enviada para $VPS_HOST"
fi
