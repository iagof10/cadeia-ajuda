export default function TechStack() {
  const techs = [
    { icon: 'fab fa-microsoft', label: '.NET 9 / Blazor' },
    { icon: 'fas fa-database', label: 'PostgreSQL' },
    { letter: 'S', label: 'SignalR' },
    { icon: 'fas fa-chart-area', label: 'Chart.js' },
    { icon: 'fab fa-bootstrap', label: 'Bootstrap 4' },
  ]

  return (
    <section className="tech-section" id="tecnologias">
      <div className="container">
        <div className="section-badge">ARQUITETURA MODERNA E ESCALÁVEL</div>
        <div className="tech-grid">
          {techs.map((tech, i) => (
            <div className="tech-card" key={i}>
              <div className="tech-icon">
                {tech.letter ? (
                  <span className="tech-letter">{tech.letter}</span>
                ) : (
                  <i className={tech.icon}></i>
                )}
              </div>
              <span>{tech.label}</span>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}
