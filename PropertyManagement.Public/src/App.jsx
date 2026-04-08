import { useEffect, useState } from 'react'
import './App.css'

const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7027').replace(/\/$/, '')
const adminUrl = import.meta.env.VITE_ADMIN_URL ?? 'https://localhost:7274/login'

const fallbackProperties = [
  {
    propertyId: 1,
    name: 'Sunrise Apts',
    address: '123 Maple St',
    unitNumber: '1A',
    monthlyRent: 1200,
    activeTenantCount: 1,
    openProjectCount: 0,
  },
  {
    propertyId: 3,
    name: 'Oak Estates',
    address: '456 Oak Ave',
    unitNumber: 'Unit 10',
    monthlyRent: 2000,
    activeTenantCount: 1,
    openProjectCount: 1,
  },
  {
    propertyId: 5,
    name: 'River View',
    address: '789 River Rd',
    unitNumber: '302',
    monthlyRent: 1500,
    activeTenantCount: 1,
    openProjectCount: 0,
  },
]

const initialForm = {
  propertyId: '',
  applicantFirstName: '',
  applicantLastName: '',
  email: '',
  phoneNumber: '',
  preferredMoveInDate: '',
  monthlyIncome: '',
  householdSize: '1',
  currentEmployer: '',
  petsDescription: '',
  notes: '',
}

function App() {
  const [properties, setProperties] = useState([])
  const [propertiesError, setPropertiesError] = useState('')
  const [form, setForm] = useState(initialForm)
  const [submitMessage, setSubmitMessage] = useState('')
  const [submitError, setSubmitError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    const controller = new AbortController()

    async function loadProperties() {
      try {
        const response = await fetch(buildApiUrl('/api/properties'), {
          signal: controller.signal,
        })

        if (!response.ok) {
          throw new Error('Live availability could not be loaded.')
        }

        const data = await response.json()
        setProperties(Array.isArray(data) ? data : [])
      } catch (error) {
        if (error.name === 'AbortError') {
          return
        }

        setPropertiesError('Showing fallback portfolio details while the live API is unavailable.')
      }
    }

    loadProperties()

    return () => controller.abort()
  }, [])

  const portfolio = properties.length > 0 ? properties : fallbackProperties
  const propertyOptions = portfolio.map((property) => ({
    ...property,
    propertyLabel: `${property.name} ${property.unitNumber}`.trim(),
    availabilityLabel: property.activeTenantCount === 0 ? 'Now leasing' : 'Waitlist open',
  }))

  const communityCount = [...new Set(propertyOptions.map((property) => property.name))].length
  const averageRent =
    propertyOptions.length > 0
      ? Math.round(
          propertyOptions.reduce((total, property) => total + Number(property.monthlyRent), 0) /
            propertyOptions.length,
        )
      : 0

  async function handleSubmit(event) {
    event.preventDefault()
    setSubmitMessage('')
    setSubmitError('')
    setIsSubmitting(true)

    try {
      const response = await fetch(buildApiUrl('/api/propertyapplications'), {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          propertyId: Number(form.propertyId),
          applicantFirstName: form.applicantFirstName.trim(),
          applicantLastName: form.applicantLastName.trim(),
          email: form.email.trim(),
          phoneNumber: form.phoneNumber.trim(),
          preferredMoveInDate: form.preferredMoveInDate,
          monthlyIncome: Number(form.monthlyIncome || 0),
          householdSize: Number(form.householdSize || 1),
          currentEmployer: form.currentEmployer.trim(),
          petsDescription: form.petsDescription.trim(),
          notes: form.notes.trim(),
          status: 'New',
        }),
      })

      if (!response.ok) {
        const message = await response.text()
        throw new Error(message || 'Application could not be submitted.')
      }

      const selectedProperty = propertyOptions.find((property) => String(property.propertyId) === form.propertyId)
      setSubmitMessage(
        `Application received for ${selectedProperty?.propertyLabel ?? 'the selected property'}. The leasing team can now review it in the admin portal.`,
      )
      setForm(initialForm)
    } catch (error) {
      setSubmitError(error instanceof Error ? error.message : 'Application could not be submitted.')
    } finally {
      setIsSubmitting(false)
    }
  }

  function handleFieldChange(event) {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  return (
    <div className="site-shell">
      <header className="site-header">
        <a className="brand" href="#top">
          Harbor PM
        </a>
        <nav className="main-nav" aria-label="Primary">
          <a href="#services">Services</a>
          <a href="#portfolio">Portfolio</a>
          <a href="#apply">Apply</a>
          <a href="#contact">Contact</a>
        </nav>
        <a className="admin-link" href={adminUrl}>
          Admin sign in
        </a>
      </header>

      <main id="top">
        <section className="hero-section">
          <div className="hero-copy">
            <p className="eyebrow">Property Management + Leasing</p>
            <h1>Modern housing operations with a cleaner public experience.</h1>
            <p className="hero-text">
              Harbor PM blends reliable building oversight, responsive maintenance coordination, and a polished leasing
              journey that helps prospects understand the company before they ever submit an application.
            </p>
            <div className="hero-actions">
              <a className="button button-primary" href="#apply">
                Apply for a building
              </a>
              <a className="button button-secondary" href="#portfolio">
                Explore properties
              </a>
            </div>
          </div>

          <aside className="hero-panel">
            <div className="metric-card">
              <span>Communities</span>
              <strong>{communityCount}</strong>
            </div>
            <div className="metric-card">
              <span>Units tracked</span>
              <strong>{propertyOptions.length}</strong>
            </div>
            <div className="metric-card">
              <span>Average rent</span>
              <strong>${averageRent.toLocaleString()}</strong>
            </div>
            <div className="hero-note">
              <p>Public site in React</p>
              <p>Admin operations in Blazor</p>
            </div>
          </aside>
        </section>

        <section className="section-grid" id="services">
          <div className="section-heading">
            <p className="eyebrow">What We Do</p>
            <h2>One company presence, two focused experiences.</h2>
          </div>
          <div className="service-grid">
            <article className="service-card">
              <h3>Leasing + applications</h3>
              <p>Prospects can browse buildings, join the waitlist, and submit a leasing application without seeing the internal admin tooling.</p>
            </article>
            <article className="service-card">
              <h3>Resident-first operations</h3>
              <p>Behind the scenes, the admin team manages rent, maintenance, invoices, and applicant follow-up from a single workspace.</p>
            </article>
            <article className="service-card">
              <h3>Portfolio visibility</h3>
              <p>Live portfolio data powers the public site, so the company story and the operational data stay connected.</p>
            </article>
          </div>
        </section>

        <section className="portfolio-section" id="portfolio">
          <div className="section-heading">
            <p className="eyebrow">Portfolio</p>
            <h2>Featured buildings and units</h2>
            <p>
              Use the public site to showcase the company visually while still letting applicants target a specific
              building or join the waitlist for upcoming openings.
            </p>
          </div>

          {propertiesError ? <div className="inline-message">{propertiesError}</div> : null}

          <div className="property-grid">
            {propertyOptions.map((property) => (
              <article className="property-card" key={property.propertyId}>
                <div className="property-card-header">
                  <div>
                    <p className="property-name">{property.name}</p>
                    <h3>{property.unitNumber}</h3>
                  </div>
                  <span className="availability-pill">{property.availabilityLabel}</span>
                </div>
                <p className="property-address">{property.address}</p>
                <div className="property-meta">
                  <div>
                    <span>Monthly rate</span>
                    <strong>${Number(property.monthlyRent).toLocaleString()}</strong>
                  </div>
                  <div>
                    <span>Open projects</span>
                    <strong>{property.openProjectCount}</strong>
                  </div>
                </div>
                <a className="property-link" href="#apply" onClick={() => setForm((current) => ({ ...current, propertyId: String(property.propertyId) }))}>
                  Apply or join waitlist
                </a>
              </article>
            ))}
          </div>
        </section>

        <section className="process-section">
          <div className="section-heading">
            <p className="eyebrow">How It Works</p>
            <h2>A cleaner leasing path for prospects and staff.</h2>
          </div>
          <div className="process-grid">
            <article>
              <span>01</span>
              <h3>Discover the company</h3>
              <p>Visitors land on a modern public site that explains what the company does and highlights the portfolio.</p>
            </article>
            <article>
              <span>02</span>
              <h3>Choose a building</h3>
              <p>Applicants select a building or unit and submit interest with real contact and move-in details.</p>
            </article>
            <article>
              <span>03</span>
              <h3>Review inside admin</h3>
              <p>The leasing team signs in through the admin entry point and reviews submissions inside the existing management portal.</p>
            </article>
          </div>
        </section>

        <section className="application-section" id="apply">
          <div className="application-copy">
            <p className="eyebrow">Applications</p>
            <h2>Apply for a building</h2>
            <p>
              This form sends applicants directly into the admin pipeline, so staff can review submissions without
              re-entering anything by hand.
            </p>
            <div className="application-side-note">
              <strong>Admin workflow</strong>
              <p>Submitted applications appear in the admin portal under the new Applications section.</p>
            </div>
          </div>

          <form className="application-form" onSubmit={handleSubmit}>
            <div className="form-grid">
              <label>
                Building or unit
                <select name="propertyId" value={form.propertyId} onChange={handleFieldChange} required>
                  <option value="">Select a property</option>
                  {propertyOptions.map((property) => (
                    <option key={property.propertyId} value={property.propertyId}>
                      {property.propertyLabel} - {property.address}
                    </option>
                  ))}
                </select>
              </label>
              <label>
                Preferred move-in date
                <input
                  name="preferredMoveInDate"
                  type="date"
                  value={form.preferredMoveInDate}
                  onChange={handleFieldChange}
                  required
                />
              </label>
              <label>
                First name
                <input name="applicantFirstName" value={form.applicantFirstName} onChange={handleFieldChange} required />
              </label>
              <label>
                Last name
                <input name="applicantLastName" value={form.applicantLastName} onChange={handleFieldChange} required />
              </label>
              <label>
                Email
                <input name="email" type="email" value={form.email} onChange={handleFieldChange} required />
              </label>
              <label>
                Phone
                <input name="phoneNumber" value={form.phoneNumber} onChange={handleFieldChange} required />
              </label>
              <label>
                Monthly income
                <input name="monthlyIncome" type="number" min="0" value={form.monthlyIncome} onChange={handleFieldChange} required />
              </label>
              <label>
                Household size
                <input name="householdSize" type="number" min="1" max="20" value={form.householdSize} onChange={handleFieldChange} required />
              </label>
              <label className="span-2">
                Current employer
                <input name="currentEmployer" value={form.currentEmployer} onChange={handleFieldChange} required />
              </label>
              <label className="span-2">
                Pets
                <input name="petsDescription" value={form.petsDescription} onChange={handleFieldChange} placeholder="Optional" />
              </label>
              <label className="span-2">
                Notes
                <textarea name="notes" rows="4" value={form.notes} onChange={handleFieldChange} placeholder="Tell us about timing, needs, or questions." />
              </label>
            </div>

            {submitMessage ? <div className="success-message">{submitMessage}</div> : null}
            {submitError ? <div className="error-message">{submitError}</div> : null}

            <button className="button button-primary form-submit" type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Submitting...' : 'Submit application'}
            </button>
          </form>
        </section>

        <section className="admin-section" id="contact">
          <div>
            <p className="eyebrow">Admin Access</p>
            <h2>Need the operations side?</h2>
            <p>
              Staff can jump from the public site straight into the secure admin interface for tenants, rent, maintenance,
              invoices, and application review.
            </p>
          </div>
          <div className="admin-actions">
            <a className="button button-primary" href={adminUrl}>
              Open admin portal
            </a>
            <a className="button button-secondary" href="#apply">
              Review leasing intake flow
            </a>
          </div>
        </section>
      </main>

      <footer className="site-footer">
        <div>
          <strong>Harbor PM</strong>
          <p>Leasing, maintenance oversight, and property operations in one connected system.</p>
        </div>
        <p>Public React frontend with a linked Blazor admin workspace.</p>
      </footer>
    </div>
  )
}

function buildApiUrl(path) {
  return apiBaseUrl ? `${apiBaseUrl}${path}` : path
}

export default App
