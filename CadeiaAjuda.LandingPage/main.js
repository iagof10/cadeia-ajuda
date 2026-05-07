/* ===================================================================
   AssistChain Landing Page — Vanilla JS
   =================================================================== */

(function () {
  'use strict';

  const { gsap } = window;
  const ScrollTrigger = window.ScrollTrigger;
  if (gsap && ScrollTrigger) gsap.registerPlugin(ScrollTrigger);

  /* ---------- Helpers ---------- */
  const qs = (s, r) => (r || document).querySelector(s);
  const qsa = (s, r) => Array.from((r || document).querySelectorAll(s));

  function ready(fn) {
    if (document.readyState !== 'loading') fn();
    else document.addEventListener('DOMContentLoaded', fn);
  }

  /* ---------- Navbar (scroll + mobile menu + app URL) ---------- */
  function initNavbar() {
    const navbar = qs('#navbar');
    const navLinks = qs('#navLinks');
    const menuBtn = qs('#mobileMenuBtn');
    const menuIcon = qs('#mobileMenuIcon');
    const enterBtn = qs('#navEnterBtn');

    // App URL switching (dev vs prod by hostname)
    if (enterBtn) {
      const host = window.location.hostname;
      const isDev = host === 'localhost' || host === '127.0.0.1' || host.includes('dev.');
      enterBtn.href = isDev
        ? 'https://app.dev.assistchain.com.br'
        : 'https://app.assistchain.com.br';
    }

    const onScroll = () => {
      if (window.scrollY > 50) navbar.classList.add('scrolled');
      else navbar.classList.remove('scrolled');
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();

    const setMenu = (open) => {
      navLinks.classList.toggle('open', open);
      if (menuIcon) {
        menuIcon.classList.toggle('fa-bars', !open);
        menuIcon.classList.toggle('fa-times', open);
      }
    };
    menuBtn.addEventListener('click', () => {
      setMenu(!navLinks.classList.contains('open'));
    });
    qsa('[data-nav-link]').forEach((a) => a.addEventListener('click', () => setMenu(false)));
  }

  /* ---------- Scroll Progress Bar ---------- */
  function initScrollProgress() {
    const bar = qs('#scrollProgressBar');
    if (!bar || !gsap || !ScrollTrigger) return;
    gsap.to(bar, {
      scaleX: 1,
      ease: 'none',
      scrollTrigger: {
        trigger: document.body,
        start: 'top top',
        end: 'bottom bottom',
        scrub: 0.3,
      },
    });
  }

  /* ---------- Hero (split text + rotating message + GSAP intro) ---------- */
  const heroMessages = [
    { line1: 'Controle total da sua', line2Start: 'operação em ', line2Highlight: 'tempo real.' },
    { line1: 'Paradas não avisam.', line2Start: 'É preciso reagir ', line2Highlight: 'na hora.' },
    { line1: 'Mais controle na fábrica.', line2Start: 'Menos ', line2Highlight: 'tempo parado.' },
  ];

  function splitChars(text) {
    const frag = document.createDocumentFragment();
    for (const ch of text) {
      const span = document.createElement('span');
      span.className = 'split-char';
      span.style.display = 'inline-block';
      span.textContent = ch === ' ' ? '\u00A0' : ch;
      frag.appendChild(span);
    }
    return frag;
  }

  function renderHeroTitle(idx) {
    const title = qs('#heroTitle');
    if (!title) return;
    const msg = heroMessages[idx];
    const line1 = title.querySelector('.hero-line-1');
    const line2 = title.querySelector('.hero-line-2');
    line1.innerHTML = '';
    line2.innerHTML = '';
    line1.appendChild(splitChars(msg.line1));
    line2.appendChild(splitChars(msg.line2Start));
    const grad = document.createElement('span');
    grad.className = 'text-gradient hero-gradient-text';
    grad.textContent = msg.line2Highlight;
    line2.appendChild(grad);
  }

  function initHero() {
    renderHeroTitle(0);
    let activeIdx = 0;

    if (gsap) {
      const tl = gsap.timeline({ defaults: { ease: 'power4.out' } });

      gsap.to('.orb-1', { x: 80, y: 50, duration: 8, repeat: -1, yoyo: true, ease: 'sine.inOut' });
      gsap.to('.orb-2', { x: -60, y: -40, duration: 10, repeat: -1, yoyo: true, ease: 'sine.inOut' });
      gsap.to('.orb-3', { x: 40, y: -50, duration: 12, repeat: -1, yoyo: true, ease: 'sine.inOut' });

      gsap.to('.hero-grid-overlay', {
        backgroundPosition: '30px 30px',
        duration: 20,
        repeat: -1,
        ease: 'none',
      });

      tl.from('.hero-badge', { y: 30, opacity: 0, scale: 0.85, duration: 0.7, ease: 'back.out(1.7)' })
        .from('.hero-line-1 .split-char', { y: 60, opacity: 0, rotateX: -80, duration: 0.5, stagger: 0.02, ease: 'power4.out' }, '-=0.15')
        .from('.hero-line-2 .split-char', { y: 60, opacity: 0, rotateX: -80, duration: 0.5, stagger: 0.02, ease: 'power4.out' }, '-=0.35')
        .from('.hero-gradient-text', { y: 40, opacity: 0, scale: 0.9, duration: 0.6, ease: 'back.out(1.7)' }, '-=0.25')
        .from('.hero-subtitle', { y: 25, opacity: 0, duration: 0.7, ease: 'power3.out' }, '-=0.3')
        .from('.hero-buttons .btn', { y: 25, scale: 0.9, duration: 0.5, stagger: 0.12, ease: 'back.out(1.7)', clearProps: 'all' }, '-=0.4')
        .from('.hero-mockup', { y: 100, opacity: 0, scale: 0.85, rotateX: 15, duration: 1.2, ease: 'power3.out' }, '-=0.6');

      gsap.to('.hero-mockup-inner', { y: -12, duration: 3, repeat: -1, yoyo: true, ease: 'sine.inOut' });

      gsap.to('.hero-badge', {
        boxShadow: '0 0 25px rgba(245,124,0,0.25), 0 0 50px rgba(245,124,0,0.08)',
        duration: 2, repeat: -1, yoyo: true, ease: 'sine.inOut', delay: 1.5,
      });

      // Hero parallax on scroll
      if (ScrollTrigger) {
        gsap.to('.hero-content', {
          y: 120, opacity: 0.2, ease: 'none',
          scrollTrigger: { trigger: '.hero', start: 'top top', end: 'bottom top', scrub: true },
        });
        gsap.to('.hero-bg-effects', {
          y: 60, scale: 1.05, ease: 'none',
          scrollTrigger: { trigger: '.hero', start: 'top top', end: 'bottom top', scrub: true },
        });
      }
    }

    // Rotation
    setInterval(() => {
      activeIdx = (activeIdx + 1) % heroMessages.length;
      renderHeroTitle(activeIdx);
      if (gsap) {
        gsap.fromTo(
          '.hero-title-swap .split-char, .hero-title-swap .hero-gradient-text',
          { y: 24, opacity: 0 },
          { y: 0, opacity: 1, duration: 0.45, stagger: 0.015, ease: 'power3.out', overwrite: true }
        );
      }
    }, 5500);
  }

  /* ---------- Particle Canvas ---------- */
  function initParticles() {
    const canvas = qs('#particleCanvas');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    let particles = [];
    let mouse = { x: -1000, y: -1000 };
    let raf = 0;

    function resize() {
      const dpr = window.devicePixelRatio || 1;
      const rect = canvas.getBoundingClientRect();
      canvas.width = rect.width * dpr;
      canvas.height = rect.height * dpr;
      ctx.setTransform(1, 0, 0, 1, 0, 0);
      ctx.scale(dpr, dpr);
    }

    function init() {
      const rect = canvas.getBoundingClientRect();
      const count = Math.min(Math.floor((rect.width * rect.height) / 8000), 120);
      particles = Array.from({ length: count }, () => ({
        x: Math.random() * rect.width,
        y: Math.random() * rect.height,
        vx: (Math.random() - 0.5) * 0.4,
        vy: (Math.random() - 0.5) * 0.4,
        radius: Math.random() * 1.5 + 0.5,
        opacity: Math.random() * 0.5 + 0.2,
      }));
    }

    function draw() {
      const rect = canvas.getBoundingClientRect();
      const w = rect.width, h = rect.height;
      ctx.clearRect(0, 0, w, h);

      for (const p of particles) {
        p.x += p.vx; p.y += p.vy;
        if (p.x < 0) p.x = w; if (p.x > w) p.x = 0;
        if (p.y < 0) p.y = h; if (p.y > h) p.y = 0;

        const dx = p.x - mouse.x, dy = p.y - mouse.y;
        const dist = Math.sqrt(dx * dx + dy * dy);
        if (dist < 150) {
          const force = (150 - dist) / 150;
          p.vx += (dx / dist) * force * 0.3;
          p.vy += (dy / dist) * force * 0.3;
        }
        p.vx *= 0.99; p.vy *= 0.99;

        ctx.beginPath();
        ctx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(245, 124, 0, ${p.opacity})`;
        ctx.fill();
      }

      const cd = 120;
      for (let i = 0; i < particles.length; i++) {
        for (let j = i + 1; j < particles.length; j++) {
          const dx = particles[i].x - particles[j].x;
          const dy = particles[i].y - particles[j].y;
          const dist = Math.sqrt(dx * dx + dy * dy);
          if (dist < cd) {
            const alpha = (1 - dist / cd) * 0.15;
            ctx.beginPath();
            ctx.moveTo(particles[i].x, particles[i].y);
            ctx.lineTo(particles[j].x, particles[j].y);
            ctx.strokeStyle = `rgba(245, 124, 0, ${alpha})`;
            ctx.lineWidth = 0.5;
            ctx.stroke();
          }
        }
      }

      if (mouse.x > 0 && mouse.y > 0) {
        for (const p of particles) {
          const dx = p.x - mouse.x, dy = p.y - mouse.y;
          const dist = Math.sqrt(dx * dx + dy * dy);
          if (dist < 200) {
            const alpha = (1 - dist / 200) * 0.3;
            ctx.beginPath();
            ctx.moveTo(p.x, p.y);
            ctx.lineTo(mouse.x, mouse.y);
            ctx.strokeStyle = `rgba(255, 152, 0, ${alpha})`;
            ctx.lineWidth = 0.8;
            ctx.stroke();
          }
        }
      }
      raf = requestAnimationFrame(draw);
    }

    resize(); init(); draw();
    window.addEventListener('resize', () => { resize(); init(); });
    canvas.addEventListener('mousemove', (e) => {
      const rect = canvas.getBoundingClientRect();
      mouse = { x: e.clientX - rect.left, y: e.clientY - rect.top };
    });
    canvas.addEventListener('mouseleave', () => { mouse = { x: -1000, y: -1000 }; });
  }

  /* ---------- Stats (animated counters) ---------- */
  const stats = [
    { value: 99.9, suffix: '%', prefix: '', icon: 'fas fa-shield-alt', label: 'Uptime Garantido', color: 'var(--green)' },
    { value: 1000000, suffix: '+', prefix: '', icon: 'fas fa-ticket-alt', label: 'Chamados Gerenciados', color: 'var(--orange)' },
    { value: 2, suffix: 's', prefix: '< ', icon: 'fas fa-bolt', label: 'Tempo de Alerta', color: 'var(--yellow)' },
    { value: 0, suffix: '', prefix: '', icon: 'fas fa-clock', label: 'Monitoramento Ativo', color: 'var(--purple)', static: '24/7' },
  ];

  function initStats() {
    const grid = qs('#statsGrid');
    if (!grid) return;
    grid.innerHTML = '';
    stats.forEach((s) => {
      const card = document.createElement('div');
      card.className = 'stat-card';
      const valSpan = s.static
        ? s.static
        : `<span data-counter data-value="${s.value}" data-prefix="${s.prefix}" data-suffix="${s.suffix}">${s.prefix}0${s.suffix}</span>`;
      card.innerHTML = `
        <div class="stat-icon-wrap" style="color: ${s.color};">
          <i class="${s.icon}"></i>
        </div>
        <h3>${valSpan}</h3>
        <p>${s.label}</p>`;
      grid.appendChild(card);
    });

    if (!gsap || !ScrollTrigger) return;

    qsa('[data-counter]').forEach((el) => {
      const value = parseFloat(el.dataset.value);
      const prefix = el.dataset.prefix || '';
      const suffix = el.dataset.suffix || '';
      let counted = false;
      ScrollTrigger.create({
        trigger: el, start: 'top 85%', once: true,
        onEnter: () => {
          if (counted) return;
          counted = true;
          const obj = { v: 0 };
          gsap.to(obj, {
            v: value, duration: 2, ease: 'power2.out',
            onUpdate: () => {
              el.textContent = prefix + (value >= 1 ? Math.floor(obj.v).toLocaleString('pt-BR') : obj.v.toFixed(1)) + suffix;
            },
          });
        },
      });
    });

    gsap.fromTo('.stat-card',
      { y: 50, opacity: 0, scale: 0.85 },
      {
        y: 0, opacity: 1, scale: 1, duration: 0.7, stagger: 0.1, ease: 'back.out(1.7)',
        scrollTrigger: { trigger: '.stats-section', start: 'top 90%', once: true },
      }
    );
  }

  /* ---------- Platform Demo (browser tabs) ---------- */
  const platformTabs = [
    { key: 'dashboard', label: 'Dashboard', image: 'image/print/AssistChain-Dashboard.png', url: 'app.assistchain.com.br/dashboard' },
    { key: 'andon', label: 'Painel Andon', image: 'image/print/AssistChain-Andon.gif', url: 'app.assistchain.com.br/andon' },
    { key: 'recursos', label: 'Plantas e Recursos', image: 'image/print/AssistChain-PlantasRecursos.png', url: 'app.assistchain.com.br/recursos' },
    { key: 'cadeia', label: 'Cadeia de Ajuda', image: 'image/print/AssistChain-CadeiadeAjuda.png', url: 'app.assistchain.com.br/cadeia' },
  ];

  function initPlatformDemo() {
    const tabsHost = qs('#browserTabs');
    const urlEl = qs('#browserUrl');
    const imgEl = qs('#browserImage');
    if (!tabsHost) return;

    let active = platformTabs[0].key;

    const render = () => {
      tabsHost.innerHTML = '';
      platformTabs.forEach((t) => {
        const btn = document.createElement('button');
        btn.className = 'browser-tab' + (t.key === active ? ' active' : '');
        btn.textContent = t.label;
        btn.addEventListener('click', () => { active = t.key; render(); });
        tabsHost.appendChild(btn);
      });
      const cur = platformTabs.find((t) => t.key === active);
      urlEl.textContent = cur.url;
      imgEl.src = cur.image;
      imgEl.alt = cur.label;
    };
    render();

    if (gsap && ScrollTrigger) {
      gsap.from('.browser-frame', {
        scrollTrigger: { trigger: '.platform-section', start: 'top 80%', once: true },
        y: 60, opacity: 0, scale: 0.94, duration: 1, ease: 'power3.out',
      });
      // Tabs: animate without hiding on initial render (avoid `from` which sets opacity 0)
      gsap.fromTo('#browserTabs .browser-tab',
        { y: -15, opacity: 0 },
        {
          y: 0, opacity: 1, duration: 0.35, stagger: 0.06, ease: 'power2.out',
          scrollTrigger: { trigger: '.platform-section', start: 'top 80%', once: true },
        }
      );
    }
  }

  /* ---------- Features (bento + 3D tilt) ---------- */
  const features = [
    { icon: 'fas fa-chart-line', color: 'icon-blue', title: 'Dashboard em Tempo Real', desc: 'KPIs, gráficos e rankings atualizados a cada segundo para decisão rápida.' },
    { icon: 'fas fa-ticket-alt', color: 'icon-green', title: 'Gestão de Chamados', desc: 'Abertura, filtragem e encerramento de chamados (Cadeia de Ajuda) com precisão.' },
    { icon: 'fas fa-tv', color: 'icon-purple', title: 'Painel Andon para TVs', desc: 'Visualização fullscreen com cronômetros, setores e alertas visuais de alto contraste.' },
    { icon: 'fas fa-sliders-h', color: 'icon-orange', title: 'Configuração por Usuário', desc: 'Personalize a experiência do Andon com sliders, switches e preferências individuais.' },
    { icon: 'fas fa-sitemap', color: 'icon-teal', title: 'Recursos Hierárquicos', desc: 'Mapeie sua planta com uma árvore ilimitada de setores, linhas e máquinas.' },
    { icon: 'fas fa-chart-bar', color: 'icon-pink', title: 'Relatórios Analíticos', desc: 'Acesso a 8 tipos de gráficos detalhados e exportação de dados para CSV.' },
    { icon: 'fas fa-level-up-alt', color: 'icon-red', title: 'Escalonamento Automático', desc: 'Defina níveis de tempo. Se não resolvido, o sistema alerta o próximo nível da gestão.' },
    { icon: 'fas fa-user-shield', color: 'icon-cyan', title: 'Gestão de Permissões', desc: 'Controle granular com 18 tipos de permissões organizadas em 7 grupos de usuários.' },
  ];

  function initFeatures() {
    const host = qs('#featuresBento');
    if (!host) return;
    host.innerHTML = '';
    features.forEach((f, i) => {
      const card = document.createElement('div');
      card.className = 'feature-card' + (i === 0 || i === 3 ? ' bento-wide' : '');
      card.style.transformStyle = 'preserve-3d';
      card.innerHTML = `
        <div class="card-glow"></div>
        <div class="feature-icon ${f.color}" style="transform: translateZ(20px);">
          <i class="${f.icon}"></i>
        </div>
        <h3 style="transform: translateZ(15px);">${f.title}</h3>
        <p style="transform: translateZ(10px);">${f.desc}</p>`;

      card.addEventListener('mousemove', (e) => {
        const rect = card.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        const rotateX = ((y - rect.height / 2) / (rect.height / 2)) * -6;
        const rotateY = ((x - rect.width / 2) / (rect.width / 2)) * 6;
        if (gsap) {
          gsap.to(card, { rotateX, rotateY, duration: 0.4, ease: 'power2.out', transformPerspective: 800 });
          const glow = card.querySelector('.card-glow');
          if (glow) gsap.to(glow, { x: x - rect.width / 2, y: y - rect.height / 2, opacity: 1, duration: 0.3 });
        }
      });
      card.addEventListener('mouseleave', () => {
        if (!gsap) return;
        gsap.to(card, { rotateX: 0, rotateY: 0, duration: 0.6, ease: 'elastic.out(1, 0.5)' });
        const glow = card.querySelector('.card-glow');
        if (glow) gsap.to(glow, { opacity: 0, duration: 0.4 });
      });

      host.appendChild(card);
    });

    if (gsap && ScrollTrigger) {
      gsap.fromTo('.feature-card',
        { y: 60, opacity: 0 },
        {
          y: 0, opacity: 1, duration: 0.7,
          stagger: { each: 0.06, from: 'start' }, ease: 'power3.out',
          scrollTrigger: { trigger: '.features-section', start: 'top 85%', once: true },
        }
      );
    }
  }

  /* ---------- Andon Showcase (live clock + scroll anims) ---------- */
  function initAndonShowcase() {
    const clockEl = qs('#liveClock');
    const update = () => {
      clockEl.textContent = new Date().toLocaleTimeString('pt-BR', {
        hour: '2-digit', minute: '2-digit', second: '2-digit',
      });
    };
    update();
    setInterval(update, 1000);

    if (!gsap || !ScrollTrigger) return;
    const tl = gsap.timeline({
      scrollTrigger: { trigger: '.andon-showcase', start: 'top 70%', once: true },
    });
    tl.from('.showcase-text h2', { x: -60, opacity: 0, duration: 0.8, ease: 'power3.out' })
      .from('.showcase-text p', { x: -40, opacity: 0, duration: 0.6, ease: 'power3.out' }, '-=0.4')
      .from('.showcase-features li', { x: -30, opacity: 0, duration: 0.4, stagger: 0.08, ease: 'power3.out' }, '-=0.3')
      .from('.showcase-text .btn', { y: 15, opacity: 0, duration: 0.4, ease: 'back.out(1.7)' }, '-=0.2')
      .from('.andon-live-panel', { x: 80, opacity: 0, rotateY: -12, duration: 1, ease: 'power3.out' }, '-=1');

    gsap.from('.andon-gif', {
      scrollTrigger: { trigger: '.andon-live-panel', start: 'top 80%', once: true },
      scale: 0.94, opacity: 0, duration: 0.8, ease: 'power3.out',
    });
  }

  /* ---------- How it Works (Timeline) ---------- */
  const steps = [
    { number: '01', icon: 'fas fa-cogs', title: 'Configure', desc: 'Mapeie sua fábrica. Cadastre setores, recursos, tempos de SLA e defina os grupos de permissões.' },
    { number: '02', icon: 'fas fa-play-circle', title: 'Opere', desc: 'Instale as TVs Andon e acesse o sistema pelo tablet ou desktop. Abra chamados com 2 cliques.' },
    { number: '03', icon: 'fas fa-chart-pie', title: 'Analise', desc: 'Acompanhe métricas, identifique gargalos recorrentes e otimize a planta com relatórios automáticos.' },
  ];

  function initTimeline() {
    const host = qs('#timeline');
    if (!host) return;
    host.innerHTML = '';
    steps.forEach((s) => {
      const item = document.createElement('div');
      item.className = 'timeline-item';
      item.innerHTML = `
        <div class="timeline-marker">
          <div class="timeline-number">${s.number}</div>
        </div>
        <div class="timeline-card">
          <div class="step-icon"><i class="${s.icon}"></i></div>
          <h3>${s.title}</h3>
          <p>${s.desc}</p>
        </div>`;
      host.appendChild(item);
    });
    const line = document.createElement('div');
    line.className = 'timeline-line';
    host.appendChild(line);

    if (!gsap || !ScrollTrigger) return;
    gsap.fromTo('.timeline-item',
      { x: -40, opacity: 0 },
      {
        x: 0, opacity: 1, duration: 0.7, stagger: 0.2, ease: 'power3.out',
        scrollTrigger: { trigger: '.how-section', start: 'top 80%', once: true },
      }
    );
    gsap.fromTo('.timeline-line',
      { scaleY: 0 },
      {
        scaleY: 1, transformOrigin: 'top center', duration: 1.2, ease: 'power3.out',
        scrollTrigger: { trigger: '.how-section', start: 'top 80%', once: true },
      }
    );
  }

  /* ---------- Tech Stack (inline SVGs) ---------- */
  const techIcons = {
    dotnet: '<svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" aria-hidden="true"><path d="M24 8.77h-2.468v7.565h-1.425V8.77H17.66V7.515H24zm-9.789 7.565h-4.696V7.515h4.503V8.77H10.94v2.687h3.7v1.247h-3.7v2.376h3.971zm-6.477 0H6.303L3.336 11.65a4.182 4.182 0 0 1-.371-.733H2.94c.04.297.058.881.058 1.751v3.667H1.667V7.515h1.519l2.86 4.586c.246.394.405.66.479.797h.018a8.34 8.34 0 0 1-.061-1.51V7.515h1.292zm-7.434-.376a.898.898 0 0 1-.65-.27.892.892 0 0 1-.273-.656.916.916 0 0 1 .923-.926.93.93 0 0 1 .67.27.898.898 0 0 1 .277.656.892.892 0 0 1-.27.656.93.93 0 0 1-.677.27Z"/></svg>',
    postgresql: '<svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" aria-hidden="true"><path d="M17.128 0a8.521 8.521 0 0 0-2.236.312l-.04.012a8.667 8.667 0 0 0-1.395.49 6.45 6.45 0 0 0-1.157-.103c-1.314-.021-2.448.296-3.342.876C8.066 1.12 4.467.36 1.797 2.027.225 3.4-.103 5.474.027 7.382c.06.91.222 1.972.54 3.135l.04.13c.342 1.142.86 2.328 1.563 3.351a4.965 4.965 0 0 0-.798 2.27c-.165 1.343.25 2.488 1.143 3.314.218.196.467.367.74.51-.01.085-.018.17-.024.255-.094 1.146.232 2.21.927 2.998.652.74 1.585 1.225 2.679 1.42 1.804.32 3.5-.087 4.405-.792.643.024 1.31-.011 1.997-.103 1.04-.137 2.043-.4 2.957-.764-.046.087-.092.176-.142.262a4.97 4.97 0 0 0-.572 1.534c-.197 1.034.07 1.998.78 2.682.72.696 1.766.96 2.872.732 1.04-.215 1.84-.85 2.244-1.737.402-.882.41-1.866.024-2.756l-.029-.066c.32-.058.643-.128.965-.21l.092-.025c.91-.243 1.91-.582 2.78-1.093 1.226-.72 2.114-1.747 2.385-2.832.354-1.413-.193-2.696-1.43-3.337l-.054-.027c.173-.413.27-.857.286-1.314.027-.778-.157-1.508-.526-2.169.085-.213.16-.43.227-.649.196-.643.275-1.337.232-2.013-.038-.572-.176-1.087-.41-1.51-.358-.65-.96-1.156-1.673-1.388-.348-.114-.738-.166-1.158-.18a8.69 8.69 0 0 0-1.063.034c-.082.008-.165.018-.247.029a8.5 8.5 0 0 0-1.58-.137zm-.012.395a8.097 8.097 0 0 1 4.448 1.317c2.252 1.5 3.118 4.247 3.118 4.247s-.866 2.748-3.118 4.247a8.085 8.085 0 0 1-4.448 1.318l.001-.001-.001.001a8.097 8.097 0 0 1-4.449-1.318C10.415 8.707 9.55 5.96 9.55 5.96s.866-2.748 3.117-4.247A8.097 8.097 0 0 1 17.116.395zm-.067.515c-.83.014-1.65.18-2.42.484-.79.31-1.516.756-2.124 1.27-.608.516-1.094 1.116-1.421 1.736-.327.62-.495 1.262-.495 1.85s.168 1.23.495 1.85c.327.62.813 1.22 1.421 1.736a7.62 7.62 0 0 0 2.123 1.27c.79.31 1.633.484 2.488.484h.13c.854 0 1.697-.174 2.487-.484a7.62 7.62 0 0 0 2.123-1.27c.608-.515 1.094-1.116 1.421-1.736.327-.62.495-1.262.495-1.85s-.168-1.23-.495-1.85c-.327-.62-.813-1.22-1.421-1.736a7.62 7.62 0 0 0-2.123-1.27c-.79-.304-1.61-.47-2.44-.484h-.244z" /></svg>',
    broadcast: '<svg viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg" aria-hidden="true"><path d="M3.05 3.05a7 7 0 0 0 0 9.9.5.5 0 0 1-.707.707 8 8 0 0 1 0-11.314.5.5 0 0 1 .707.707zm2.122 2.122a4 4 0 0 0 0 5.656.5.5 0 1 1-.708.708 5 5 0 0 1 0-7.072.5.5 0 0 1 .708.708zm5.656-.708a.5.5 0 0 1 .708 0 5 5 0 0 1 0 7.072.5.5 0 1 1-.708-.708 4 4 0 0 0 0-5.656.5.5 0 0 1 0-.708zm2.122-2.12a.5.5 0 0 1 .707 0 8 8 0 0 1 0 11.313.5.5 0 0 1-.707-.707 7 7 0 0 0 0-9.9.5.5 0 0 1 0-.707zM10 8a2 2 0 1 1-4 0 2 2 0 0 1 4 0z"/></svg>',
    chartdotjs: '<svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" aria-hidden="true"><path d="M11.945 0C5.467 0 .19 5.224.066 11.703l11.88-7.876v-.022l11.987 7.946C23.74 5.222 18.366 0 11.946 0zm0 4.235l-9.954 6.6 2.85 5.247 7.106-4.713 8.106 5.376 1.95-3.59zm0 8.293l-7.108 4.713 7.108 13.005 8.106-14.917z"/></svg>',
    bootstrap: '<svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" aria-hidden="true"><path d="M11.77 11.24H9.956V8.202h2.152c1.17 0 1.834.522 1.834 1.466 0 1.008-.773 1.572-2.174 1.572zm.324 1.206H9.957v3.348h2.231c1.459 0 2.232-.585 2.232-1.685s-.795-1.663-2.326-1.663zM24 11.39v1.218c-1.128.108-1.817.944-2.226 2.268-.407 1.319-.463 2.937-.42 4.186.045 1.3-.968 2.5-2.337 2.5H4.985c-1.37 0-2.383-1.2-2.337-2.5.043-1.249-.013-2.867-.42-4.186-.41-1.324-1.1-2.16-2.228-2.268V11.39c1.128-.108 1.819-.944 2.227-2.268.408-1.319.464-2.937.42-4.186-.045-1.3.968-2.5 2.338-2.5h14.032c1.37 0 2.382 1.2 2.337 2.5-.043 1.249.013 2.867.42 4.186.409 1.324 1.098 2.16 2.226 2.268zm-7.927 2.817c0-1.354-.953-2.333-2.368-2.488v-.057c1.04-.169 1.856-1.135 1.856-2.213 0-1.537-1.213-2.538-3.062-2.538h-4.16v10.172h4.181c2.218 0 3.553-1.086 3.553-2.876z"/></svg>',
  };

  const techs = [
    { iconKey: 'dotnet', label: '.NET 9 / Blazor', color: '#512bd4' },
    { iconKey: 'postgresql', label: 'PostgreSQL', color: '#336791' },
    { iconKey: 'broadcast', label: 'SignalR', color: '#7c3aed' },
    { iconKey: 'chartdotjs', label: 'Chart.js', color: '#ff6384' },
    { iconKey: 'bootstrap', label: 'Bootstrap 4', color: '#7952b3' },
  ];

  function initTechStack() {
    const host = qs('#techGrid');
    if (!host) return;
    host.innerHTML = '';
    techs.forEach((t) => {
      const card = document.createElement('div');
      card.className = 'tech-card';
      card.innerHTML = `
        <div class="tech-icon" style="--tech-color: ${t.color};">
          ${techIcons[t.iconKey] || ''}
        </div>
        <span>${t.label}</span>`;
      host.appendChild(card);
    });

    if (gsap && ScrollTrigger) {
      gsap.fromTo('.tech-card',
        { y: 30, opacity: 0, scale: 0.92 },
        {
          y: 0, opacity: 1, scale: 1, duration: 0.5,
          stagger: { each: 0.06, from: 'center' }, ease: 'back.out(1.5)',
          scrollTrigger: { trigger: '.tech-section', start: 'top 90%', once: true },
        }
      );
    }
  }

  /* ---------- Generic Section Animations + CTA + Footer + Magnetic ---------- */
  function initSectionAnimations() {
    if (!gsap || !ScrollTrigger) return;

    gsap.from('.cta-card', {
      scrollTrigger: { trigger: '.cta-section', start: 'top 80%', once: true },
      y: 50, opacity: 0, scale: 0.9, duration: 1, ease: 'power3.out',
    });

    qsa('.section-title').forEach((el) => {
      gsap.from(el, {
        scrollTrigger: { trigger: el, start: 'top 85%', once: true },
        y: 35, opacity: 0, duration: 0.8, ease: 'power3.out',
      });
    });

    qsa('.section-badge').forEach((el) => {
      gsap.from(el, {
        scrollTrigger: { trigger: el, start: 'top 88%', once: true },
        y: 15, opacity: 0, letterSpacing: '6px', duration: 0.7, ease: 'power3.out',
      });
    });

    qsa('.section-subtitle').forEach((el) => {
      gsap.from(el, {
        scrollTrigger: { trigger: el, start: 'top 85%', once: true },
        y: 20, opacity: 0, duration: 0.7, delay: 0.1, ease: 'power3.out',
      });
    });

    gsap.from('.footer-content', {
      scrollTrigger: { trigger: '.footer', start: 'top 90%', once: true },
      y: 25, opacity: 0, duration: 0.6, ease: 'power3.out',
    });

    qsa('.btn-magnetic').forEach((btn) => {
      btn.addEventListener('mousemove', (e) => {
        const rect = btn.getBoundingClientRect();
        const x = e.clientX - rect.left - rect.width / 2;
        const y = e.clientY - rect.top - rect.height / 2;
        gsap.to(btn, { x: x * 0.25, y: y * 0.25, duration: 0.3, ease: 'power2.out' });
      });
      btn.addEventListener('mouseleave', () => {
        gsap.to(btn, { x: 0, y: 0, duration: 0.5, ease: 'elastic.out(1, 0.3)' });
      });
    });
  }

  /* ---------- Boot ---------- */
  function refreshTriggers() {
    if (ScrollTrigger) ScrollTrigger.refresh();
  }

  ready(() => {
    initNavbar();
    initScrollProgress();
    initStats();
    initPlatformDemo();
    initFeatures();
    initAndonShowcase();
    initTimeline();
    initTechStack();
    initHero();              // hero last so its intro runs after sections exist
    initParticles();
    initSectionAnimations();

    // Recalculate ScrollTrigger positions after layout has fully settled.
    // Without this, large images (hero dashboard, Andon GIF) load after JS
    // runs and push sections downward, leaving triggers anchored to stale
    // positions — which causes the entrance animations to never fire.
    requestAnimationFrame(refreshTriggers);
    window.addEventListener('load', () => {
      refreshTriggers();
      // Run twice: some browsers report `load` before late paints settle.
      setTimeout(refreshTriggers, 200);
      setTimeout(refreshTriggers, 800);
    });
    qsa('img').forEach((img) => {
      if (img.complete) return;
      img.addEventListener('load', refreshTriggers, { once: true });
      img.addEventListener('error', refreshTriggers, { once: true });
    });
    if (document.fonts && document.fonts.ready) {
      document.fonts.ready.then(refreshTriggers).catch(() => {});
    }
  });
})();
