import { useEffect, useRef, useState } from 'react'
import gsap from 'gsap'
import ParticleCanvas from './ParticleCanvas'

function SplitText({ text, className }: { text: string; className?: string }) {
  return (
    <span className={className}>
      {text.split('').map((char, i) => (
        <span key={i} className="split-char" style={{ display: 'inline-block' }}>
          {char === ' ' ? '\u00A0' : char}
        </span>
      ))}
    </span>
  )
}

const heroMessages = [
  {
    line1: 'Controle total da sua',
    line2Start: 'operação em ',
    line2Highlight: 'tempo real.',
  },
  {
    line1: 'Paradas não avisam.',
    line2Start: 'É preciso reagir ',
    line2Highlight: 'na hora.',
  },
  {
    line1: 'Mais controle na fábrica.',
    line2Start: 'Menos ',
    line2Highlight: 'tempo parado.',
  },
]

export default function Hero() {
  const sectionRef = useRef<HTMLElement>(null)
  const [activeMessageIndex, setActiveMessageIndex] = useState(0)
  const activeMessage = heroMessages[activeMessageIndex]

  useEffect(() => {
    const rotationInterval = window.setInterval(() => {
      setActiveMessageIndex((prev) => (prev + 1) % heroMessages.length)
    }, 5500)

    return () => window.clearInterval(rotationInterval)
  }, [])

  useEffect(() => {
    const ctx = gsap.context(() => {
      const tl = gsap.timeline({ defaults: { ease: 'power4.out' } })

      // Orb continuous floating
      gsap.to('.orb-1', { x: 80, y: 50, duration: 8, repeat: -1, yoyo: true, ease: 'sine.inOut' })
      gsap.to('.orb-2', { x: -60, y: -40, duration: 10, repeat: -1, yoyo: true, ease: 'sine.inOut' })
      gsap.to('.orb-3', { x: 40, y: -50, duration: 12, repeat: -1, yoyo: true, ease: 'sine.inOut' })

      gsap.to('.hero-grid-overlay', {
        backgroundPosition: '30px 30px',
        duration: 20,
        repeat: -1,
        ease: 'none',
      })

      // Badge
      tl.from('.hero-badge', {
        y: 30, opacity: 0, scale: 0.85,
        duration: 0.7,
        ease: 'back.out(1.7)',
      })

      // Title reveal char-by-char
      .from('.hero-line-1 .split-char', {
        y: 60, opacity: 0, rotateX: -80,
        duration: 0.5,
        stagger: 0.02,
        ease: 'power4.out',
      }, '-=0.15')
      .from('.hero-line-2 .split-char', {
        y: 60, opacity: 0, rotateX: -80,
        duration: 0.5,
        stagger: 0.02,
        ease: 'power4.out',
      }, '-=0.35')
      .from('.hero-gradient-text', {
        y: 40, opacity: 0, scale: 0.9,
        duration: 0.6,
        ease: 'back.out(1.7)',
      }, '-=0.25')

      // Subtitle
      .from('.hero-subtitle', {
        y: 25, opacity: 0,
        duration: 0.7,
        ease: 'power3.out',
      }, '-=0.3')

      // Buttons
      .from('.hero-buttons .btn', {
        y: 25, scale: 0.9,
        duration: 0.5,
        stagger: 0.12,
        ease: 'back.out(1.7)',
        clearProps: 'all',
      }, '-=0.4')

      // Floating dashboard mockup
      .from('.hero-mockup', {
        y: 100, opacity: 0, scale: 0.85, rotateX: 15,
        duration: 1.2,
        ease: 'power3.out',
      }, '-=0.6')

      // Continuous mockup float
      gsap.to('.hero-mockup-inner', {
        y: -12,
        duration: 3,
        repeat: -1,
        yoyo: true,
        ease: 'sine.inOut',
      })

      // Badge glow
      gsap.to('.hero-badge', {
        boxShadow: '0 0 25px rgba(245,124,0,0.25), 0 0 50px rgba(245,124,0,0.08)',
        duration: 2,
        repeat: -1,
        yoyo: true,
        ease: 'sine.inOut',
        delay: 1.5,
      })
    }, sectionRef)

    return () => ctx.revert()
  }, [])

  useEffect(() => {
    const ctx = gsap.context(() => {
      gsap.fromTo(
        '.hero-title-swap .split-char, .hero-title-swap .hero-gradient-text',
        {
          y: 24,
          opacity: 0,
        },
        {
          y: 0,
          opacity: 1,
          duration: 0.45,
          stagger: 0.015,
          ease: 'power3.out',
          overwrite: true,
        }
      )
    }, sectionRef)

    return () => ctx.revert()
  }, [activeMessageIndex])

  return (
    <section className="hero" ref={sectionRef}>
      <div className="hero-bg-effects">
        <div className="hero-gradient-orb orb-1"></div>
        <div className="hero-gradient-orb orb-2"></div>
        <div className="hero-gradient-orb orb-3"></div>
        <div className="hero-grid-overlay"></div>
        <ParticleCanvas />
      </div>
      <div className="container hero-content">
        <div className="hero-top">
          <div className="hero-badge">
            <span className="hero-badge-dot"></span>
            <span>Plataforma Industrial de Cadeia de Ajuda</span>
          </div>
          <h1 key={activeMessageIndex} className="hero-title-swap">
            <span className="hero-line-1">
              <SplitText text={activeMessage.line1} />
            </span>
            <span className="hero-line-2">
              <SplitText text={activeMessage.line2Start} />
              <span className="text-gradient hero-gradient-text">{activeMessage.line2Highlight}</span>
            </span>
          </h1>
          <p className="hero-subtitle">
            O sistema Andon definitivo. Controle em tempo real, escalonamento
            automático e relatórios precisos para operações que não podem parar.
          </p>
          <div className="hero-buttons">
            <a href="https://wa.me/5511924692776" target="_blank" rel="noreferrer" className="btn btn-primary btn-lg btn-magnetic">
              Orçamento
              <i className="fas fa-arrow-right"></i>
            </a>
            <a href="#plataforma" className="btn btn-ghost btn-lg btn-magnetic">
              <i className="fas fa-play-circle"></i>
              Ver Demonstração
            </a>
          </div>
        </div>
        <div className="hero-mockup">
          <div className="hero-mockup-inner">
            <div className="hero-mockup-glow"></div>
            <img src="/image/print/AssistChain-Dashboard.png" alt="AssistChain Dashboard" />
          </div>
        </div>
      </div>
    </section>
  )
}
