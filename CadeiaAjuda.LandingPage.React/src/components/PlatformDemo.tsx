import { useState } from 'react'

type TabKey = 'dashboard' | 'andon' | 'recursos' | 'cadeia'

const tabs: { key: TabKey; label: string; image: string; url: string }[] = [
  { key: 'dashboard', label: 'Dashboard', image: '/image/print/AssistChain-Dashboard.png', url: 'app.assistchain.com.br/dashboard' },
  { key: 'andon', label: 'Painel Andon', image: '/image/print/AssistChain-Andon.gif', url: 'app.assistchain.com.br/andon' },
  { key: 'recursos', label: 'Plantas e Recursos', image: '/image/print/AssistChain-PlantasRecursos.png', url: 'app.assistchain.com.br/recursos' },
  { key: 'cadeia', label: 'Cadeia de Ajuda', image: '/image/print/AssistChain-CadeiadeAjuda.png', url: 'app.assistchain.com.br/cadeia' },
]

export default function PlatformDemo() {
  const [activeTab, setActiveTab] = useState<TabKey>('dashboard')
  const current = tabs.find((t) => t.key === activeTab)!

  return (
    <section className="platform-section" id="plataforma">
      <div className="container">
        <div className="section-badge">PLATAFORMA REAL</div>
        <h2 className="section-title">Veja o AssistChain em ação</h2>
        <p className="section-subtitle">
          Interface limpa, funcional e projetada para o dia a dia industrial.
        </p>

        <div className="platform-demo">
          <div className="browser-frame">
            <div className="browser-header">
              <div className="browser-tabs">
                {tabs.map((tab) => (
                  <button
                    key={tab.key}
                    className={`browser-tab${activeTab === tab.key ? ' active' : ''}`}
                    onClick={() => setActiveTab(tab.key)}
                  >
                    {tab.label}
                  </button>
                ))}
              </div>
              <div className="browser-url">
                <i className="fas fa-lock"></i>
                <span>{current.url}</span>
              </div>
            </div>
            <div className="browser-content">
              <img
                src={current.image}
                alt={current.label}
                className="platform-screenshot"
              />
            </div>
          </div>
          <p className="platform-caption">
            Painel Andon ao vivo: cards de chamados com cronômetros, alertas
            visuais e carrossel automático para TVs industriais.
          </p>
        </div>
      </div>
    </section>
  )
}
