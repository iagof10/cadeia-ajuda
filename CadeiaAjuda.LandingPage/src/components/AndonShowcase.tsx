import { memo } from 'react'
import { useClock } from '../hooks/useClock'

function AndonShowcaseInner() {
  const clock = useClock()

  return (
    <section className="andon-showcase">
      <div className="container">
        <div className="showcase-grid">
          <div className="showcase-text">
            <h2>Visibilidade de ponta a ponta na fábrica</h2>
            <p>
              Transforme qualquer TV em um centro de comando operacional.
              Identifique gargalos, acompanhe tempos de resposta e garanta que
              nenhuma máquina fique parada esperando suporte.
            </p>
            <ul className="showcase-features">
              <li>
                <i className="fas fa-check-circle"></i>
                <span>Alertas sonoros e visuais customizáveis</span>
              </li>
              <li>
                <i className="fas fa-check-circle"></i>
                <span>Temporizadores de SLA em tempo real</span>
              </li>
              <li>
                <i className="fas fa-check-circle"></i>
                <span>Layout adaptativo para diferentes telas</span>
              </li>
            </ul>
            <a href="#plataforma" className="btn btn-primary">
              Explore o Painel Andon
              <i className="fas fa-arrow-right"></i>
            </a>
          </div>
          <div className="showcase-visual">
            <div className="andon-live-panel">
              <div className="live-panel-header">
                <span>Painel Andon em funcionamento</span>
                <span className="live-indicator">
                  <i className="fas fa-circle"></i> ONLINE
                </span>
                <span className="live-clock">{clock}</span>
              </div>
              <div className="andon-gif-wrap">
                <img
                  src="/image/print/AssistChain-Andon.gif"
                  alt="Painel Andon em funcionamento"
                  className="andon-gif"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}

const AndonShowcase = memo(AndonShowcaseInner)
export default AndonShowcase
