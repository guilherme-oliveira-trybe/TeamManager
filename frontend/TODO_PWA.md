# 📱 PWA - Pontos de Atenção e Melhorias

## ⚠️ Pendências Técnicas

### 1. 🎨 **Ícones Desproporcionais** (Alta Prioridade)

**Problema:** Os ícones PWA estão usando a imagem original do logo sem resize adequado, ficando desproporcionais na tela inicial do celular.

**Causa:** Simplesmente copiamos o `logo-galo.jpg` para todos os tamanhos sem processar/redimensionar.

**Solução:**
- [ ] Usar ferramenta de design (Figma, Photoshop) ou ImageMagick para gerar ícones quadrados
- [ ] Criar versões otimizadas em cada tamanho (72x72, 96x96, 128x128, 144x144, 152x152, 192x192, 384x384, 512x512)
- [ ] Garantir que o logo esteja centralizado e com padding adequado
- [ ] Testar "maskable" icons para Android (círculo adaptativo)

**Ferramentas sugeridas:**
```bash
# Instalar ImageMagick
sudo apt install imagemagick

# Script para gerar ícones proporcionais (ajustar depois)
./generate-icons.sh
```

---

### 2. 🌐 **Abrindo no Navegador em Vez de Standalone**

**Problema:** Ao clicar no ícone instalado, o app abre no navegador Chrome normal em vez do modo standalone (sem barra de endereço).

**Causa Provável:** 
- Service Worker não foi gerado corretamente (PWA desabilitado em development)
- Build não foi feito com `NODE_ENV=production`
- Manifest pode não estar sendo servido corretamente

**Diagnóstico:**
```bash
# Verificar se service workers foram gerados
ls public/sw.js public/workbox-*.js

# Verificar no Chrome DevTools (F12):
# - Application → Manifest (deve carregar)
# - Application → Service Workers (deve mostrar registrado)
```

**Solução:**
- [ ] Fazer build com `NODE_ENV=production yarn build`
- [ ] Verificar que `display: "standalone"` está no manifest.json ✅ (já está)
- [ ] Testar instalação com build de produção real
- [ ] Considerar usar HTTPS local para PWA completo (mkcert)

**Comandos para testar:**
```bash
# Build de produção
NODE_ENV=production yarn build

# Iniciar produção
yarn start

# Acessar e instalar novamente
# http://192.168.68.52:3000
```

---

### 3. 🔧 **Melhorias Futuras PWA**

Quando tivermos telas funcionais:

- [ ] **Splash Screen:** Criar splash screen personalizado com logo do Galo
- [ ] **Offline Page:** Página customizada quando usuário está offline
- [ ] **Push Notifications:** Notificações de eventos/atualizações
- [ ] **Background Sync:** Sincronizar dados quando voltar online
- [ ] **Install Prompt:** Banner customizado de instalação
- [ ] **Update Notification:** Avisar usuário quando houver nova versão

---

## 📋 Checklist PWA Completo

### Básico (Fase 1) ✅
- [x] Manifest.json configurado
- [x] Ícones em múltiplos tamanhos (gerados)
- [x] Theme color definido (#FFD700 - Gold)
- [x] Display mode standalone
- [x] Service worker (via next-pwa)
- [x] Meta tags PWA no layout

### Pendente
- [ ] Ícones com proporção correta
- [ ] Service worker funcionando (build produção)
- [ ] Modo standalone validado
- [ ] Favicon.ico customizado (atualmente é o logo.jpg)

### Avançado (Futuro)
- [ ] Manifest maskable icons (Android)
- [ ] Splash screens personalizados
- [ ] Offline fallback page
- [ ] Install prompt customizado
- [ ] Push notifications
- [ ] Background sync
- [ ] Share target API
- [ ] Shortcuts (atalhos no ícone)

---

## 🎯 Próximos Passos

**Quando implementarmos as primeiras telas:**

1. **Gerar Ícones Corretos:**
   - Contratar designer OU
   - Usar ferramenta online (realfavicongenerator.net) OU
   - Processar com ImageMagick

2. **Build de Produção Real:**
   ```bash
   NODE_ENV=production yarn build
   yarn start
   ```

3. **Validar PWA:**
   - Lighthouse (Chrome DevTools)
   - PWA Score > 90
   - Modo standalone funcionando
   - Service worker instalado

4. **Deploy:**
   - HTTPS obrigatório para PWA completo
   - Configurar domínio (ex: app.galofutebolamericano.com.br)
   - SSL/TLS configurado

---

**Observações:**
- PWA funciona melhor com HTTPS (mesmo em local com mkcert)
- Chrome é mais criterioso que outros browsers para PWA
- Testar em múltiplos dispositivos (Android, iOS tem limitações)
- iOS Safari tem suporte limitado a PWA (sem push notifications)
