export default function HowItWorks() {
  const steps = [
    {
      number: '01',
      icon: 'fas fa-cogs',
      title: 'Configure',
      desc: 'Mapeie sua fábrica. Cadastre setores, recursos, tempos de SLA e defina os grupos de permissões.',
    },
    {
      number: '02',
      icon: 'fas fa-play-circle',
      title: 'Opere',
      desc: 'Instale as TVs Andon e acesse o sistema pelo tablet ou desktop. Abra chamados com 2 cliques.',
    },
    {
      number: '03',
      icon: 'fas fa-chart-pie',
      title: 'Analise',
      desc: 'Acompanhe métricas, identifique gargalos recorrentes e otimize a planta com relatórios automáticos.',
    },
  ]

  return (
    <section className="how-section" id="como-funciona">
      <div className="container">
        <div className="section-badge">COMO FUNCIONA</div>
        <h2 className="section-title">Três passos para transformar sua operação</h2>
        <p className="section-subtitle">
          Um fluxo de trabalho projetado para simplicidade de implantação e
          robustez na operação.
        </p>

        <div className="timeline">
          {steps.map((step, i) => (
            <div className="timeline-item" key={i}>
              <div className="timeline-marker">
                <div className="timeline-number">{step.number}</div>
              </div>
              <div className="timeline-card">
                <div className="step-icon">
                  <i className={step.icon}></i>
                </div>
                <h3>{step.title}</h3>
                <p>{step.desc}</p>
              </div>
            </div>
          ))}
          <div className="timeline-line"></div>
        </div>
      </div>
    </section>
  )
}
