import { BsBroadcast } from 'react-icons/bs'
import { SiBootstrap, SiChartdotjs, SiDotnet, SiPostgresql } from 'react-icons/si'

export default function TechStack() {
  const techs = [
    { icon: SiDotnet, label: '.NET 9 / Blazor', color: '#512bd4' },
    { icon: SiPostgresql, label: 'PostgreSQL', color: '#336791' },
    { icon: BsBroadcast, label: 'SignalR', color: '#7c3aed' },
    { icon: SiChartdotjs, label: 'Chart.js', color: '#ff6384' },
    { icon: SiBootstrap, label: 'Bootstrap 4', color: '#7952b3' },
  ]

  return (
    <section className="tech-section" id="tecnologias">
      <div className="container">
        <div className="section-badge">ARQUITETURA MODERNA E ESCALÁVEL</div>
        <div className="tech-grid">
          {techs.map((tech, i) => {
            const Icon = tech.icon

            return (
              <div className="tech-card" key={i}>
                <div className="tech-icon" style={{ ['--tech-color' as string]: tech.color }}>
                  <Icon aria-hidden="true" />
                </div>
                <span>{tech.label}</span>
              </div>
            )
          })}
        </div>
      </div>
    </section>
  )
}
