import { useCallback } from 'react'
import gsap from 'gsap'

function FeatureCard({ icon, color, title, desc, className = '' }: { icon: string; color: string; title: string; desc: string; className?: string }) {
  const handleMouseMove = useCallback((e: React.MouseEvent<HTMLDivElement>) => {
    const card = e.currentTarget
    const rect = card.getBoundingClientRect()
    const x = e.clientX - rect.left
    const y = e.clientY - rect.top
    const centerX = rect.width / 2
    const centerY = rect.height / 2
    const rotateX = ((y - centerY) / centerY) * -6
    const rotateY = ((x - centerX) / centerX) * 6

    gsap.to(card, {
      rotateX,
      rotateY,
      duration: 0.4,
      ease: 'power2.out',
      transformPerspective: 800,
    })

    const glow = card.querySelector('.card-glow') as HTMLElement
    if (glow) {
      gsap.to(glow, { x: x - rect.width / 2, y: y - rect.height / 2, opacity: 1, duration: 0.3 })
    }
  }, [])

  const handleMouseLeave = useCallback((e: React.MouseEvent<HTMLDivElement>) => {
    const card = e.currentTarget
    gsap.to(card, { rotateX: 0, rotateY: 0, duration: 0.6, ease: 'elastic.out(1, 0.5)' })
    const glow = card.querySelector('.card-glow') as HTMLElement
    if (glow) gsap.to(glow, { opacity: 0, duration: 0.4 })
  }, [])

  return (
    <div
      className={`feature-card ${className}`}
      onMouseMove={handleMouseMove}
      onMouseLeave={handleMouseLeave}
      style={{ transformStyle: 'preserve-3d' }}
    >
      <div className="card-glow"></div>
      <div className={`feature-icon ${color}`} style={{ transform: 'translateZ(20px)' }}>
        <i className={icon}></i>
      </div>
      <h3 style={{ transform: 'translateZ(15px)' }}>{title}</h3>
      <p style={{ transform: 'translateZ(10px)' }}>{desc}</p>
    </div>
  )
}

const features = [
  { icon: 'fas fa-chart-line', color: 'icon-blue', title: 'Dashboard em Tempo Real', desc: 'KPIs, gráficos e rankings atualizados a cada segundo para decisão rápida.' },
  { icon: 'fas fa-ticket-alt', color: 'icon-green', title: 'Gestão de Chamados', desc: 'Abertura, filtragem e encerramento de chamados (Cadeia de Ajuda) com precisão.' },
  { icon: 'fas fa-tv', color: 'icon-purple', title: 'Painel Andon para TVs', desc: 'Visualização fullscreen com cronômetros, setores e alertas visuais de alto contraste.' },
  { icon: 'fas fa-sliders-h', color: 'icon-orange', title: 'Configuração por Usuário', desc: 'Personalize a experiência do Andon com sliders, switches e preferências individuais.' },
  { icon: 'fas fa-sitemap', color: 'icon-teal', title: 'Recursos Hierárquicos', desc: 'Mapeie sua planta com uma árvore ilimitada de setores, linhas e máquinas.' },
  { icon: 'fas fa-chart-bar', color: 'icon-pink', title: 'Relatórios Analíticos', desc: 'Acesso a 8 tipos de gráficos detalhados e exportação de dados para CSV.' },
  { icon: 'fas fa-level-up-alt', color: 'icon-red', title: 'Escalonamento Automático', desc: 'Defina níveis de tempo. Se não resolvido, o sistema alerta o próximo nível da gestão.' },
  { icon: 'fas fa-user-shield', color: 'icon-cyan', title: 'Gestão de Permissões', desc: 'Controle granular com 18 tipos de permissões organizadas em 7 grupos de usuários.' },
]

export default function Features() {
  return (
    <section className="features-section" id="funcionalidades">
      <div className="container">
        <div className="section-badge">FUNCIONALIDADES</div>
        <h2 className="section-title">Tudo o que sua produção precisa</h2>
        <p className="section-subtitle">
          Um conjunto completo de ferramentas projetado para visibilidade total e
          resposta imediata.
        </p>

        <div className="features-bento">
          {features.map((f, i) => (
            <FeatureCard
              key={i}
              icon={f.icon}
              color={f.color}
              title={f.title}
              desc={f.desc}
              className={i === 0 ? 'bento-wide' : i === 3 ? 'bento-wide' : ''}
            />
          ))}
        </div>
      </div>
    </section>
  )
}
