import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import ReportMain from '../components/ReportMain/ReportMain';
import Testimonials from '../components/Testimonials/Testimonials';
import Critical from '../components/CriticalFindings/CriticalFinding';
import Featured from '../components/Featured/Featured';
import Price from '../components/Pricing/Pricing';
import Head from '@docusaurus/Head';

import Heading from '@theme/Heading';
import styles from './index.module.css';

function HomepageHeader() {
  // const {siteConfig} = useDocusaurusContext();
  return (
    <div>
      <HomepageHeader />
    </div>
    // <header className={clsx('hero hero--primary', styles.heroBanner)}>
    //   <div className="container">
    //     
    //     <Heading as="h1" className="hero__title">
    //       {siteConfig.title}
    //     </Heading>
    //     <p className="hero__subtitle">{siteConfig.tagline}</p>
    //     <div className={styles.buttons}>
    //       <Link
    //         className="button button--secondary button--lg"
    //         to="/docs">
    //         Docusaurus Tutorial - 5min ⏱️
    //       </Link>
    //     </div>
    //   </div>
    // </header>
  );
}

// Create component that adds og:image for frontpage
// https://docusaurus.io/docs/next/docusaurus-core#head
const MySEO = () => (
  <Head>
    <meta property='og:image' content='/img/graphical-interface-of-vulnerability-scanner-with-logo.jpg' />
  </Head>
);

export default function Home() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={`OPC UA Vulnerability Scanner - ${siteConfig.title}`}
      description="Unmatched visibility to security posture of OPC UA applications, and corrective actions to keep them secure. For hackers and engineers alike."
      >
        <MySEO />
      <main>
        <ReportMain />
        <Testimonials />
        <Critical />
        <Featured />
        <Price />
      </main>
    </Layout>
  );
}
