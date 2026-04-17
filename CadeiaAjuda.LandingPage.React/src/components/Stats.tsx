import { useEffect, useRef } from 'react'
import gsap from 'gsap'
import { ScrollTrigger } from 'gsap/ScrollTrigger'

gsap.registerPlugin(ScrollTrigger)

function AnimatedCounter({ value, suffix = '', prefix = '' }: { value: number; suffix?: string; prefix?: string }) {
  const ref = useRef<HTMLSpanElement>(null)
  const counted = useRef(false)

  useEffect(() => {
    const el = ref.current
    if (!el) return

    ScrollTrigger.create({
      trigger: el,
      start: 'top 85%',
      once: true,
      onEnter: () => {
        if (counted.current) return
        counted.current = true
        const obj = { val: 0 }
        gsap.to(obj, {
          val: value,
          duration: 2,
          ease: 'power2.out',
          onUpdate: () => {
            el.textContent = prefix + (value >= 1 ? Math.floor(obj.val).toLocaleString('pt-BR') : obj.val.toFixed(1)) + suffix
          },
        })
      },
    })
  }, [value, suffix, prefix])

  return <span ref={ref}>{prefix}0{suffix}</span>
}

const stats = [
  { value: 99.9, suffix: '%', prefix: '', icon: 'fas fa-shield-alt', label: 'Uptime Garantido', color: 'var(--green)' },
  { value: 1000000, suffix: '+', prefix: '', icon: 'fas fa-ticket-alt', label: 'Chamados Gerenciados', color: 'var(--orange)' },
  { value: 2, suffix: 's', prefix: '< ', icon: 'fas fa-bolt', label: 'Tempo de Alerta', color: 'var(--yellow)' },
  { value: 0, suffix: '', prefix: '', icon: 'fas fa-clock', label: 'Monitoramento Ativo', color: 'var(--purple)', static: '24/7' },
]

export default function Stats() {
  return (
    <section className="stats-section">
      <div className="container">
        <div className="stats-grid">
          {stats.map((s, i) => (
            <div className="stat-card" key={i}>
              <div className="stat-icon-wrap" style={{ color: s.color }}>
                <i className={s.icon}></i>
              </div>
              <h3>
                {s.static ? s.static : <AnimatedCounter value={s.value} suffix={s.suffix} prefix={s.prefix} />}
              </h3>
              <p>{s.label}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}
