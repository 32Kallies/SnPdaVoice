import React, { useState } from 'react';
import ReactModal from 'react-modal';
import Modal from 'react-modal';

Modal.setAppElement('#root');

function App() {
  return (
    <>
      <TopBar />
      <div id="overlays" />
      <div id="page-content">
        <SiteHeader />
        <main>
          <GenerateMainSection />
        </main>
        <hr/>
        <SiteBio />
      </div>
      <SiteFooter />
    </>
  );
}

function TopBar() {
  return (
  <div className="topnav">
    <h2 className="top-header">Subnautica PDA Voice Generator by Kallie23</h2>
    <div className="nav-links">
      <a href="https://ko-fi.com/lee23" target="_blank" rel="noopener noreferrer">
        <img
          src="https://cdn.ko-fi.com/cdn/kofi5.png"
          alt="Buy Me a Coffee at ko-fi.com"
          height="36"
          style={{ border: '0' }}
        />
      </a>
    <a href="https://github.com/32Kallies/SnPdaVoice">Source code</a>
    </div>
  </div>
  );
}

function SiteBio() {
  return (
    <div className="container">
      <section className="bio">
        <p>As someone who has been modding Subnautica for 7+ years, I have always wanted a free, accessible and quick way to
          generate the Subnautica PDA voice. AI-generated options generally do not sound accurate for replicating the filters. So, I created
          this page to allow anyone to create their own custom PDA lines.
        </p>
        <p>I first started figuring out the PDA voice for <a href="https://www.nexusmods.com/subnautica/mods/1820">The Red Plague</a>,
        which made establishing new, natural-feeling content so much easier. The technology I use for the Red Plague PDA might be slightly
        more accurate, but I tend to use this site now because it's much faster.
        </p>
        <p>Getting the filters right has been difficult, taking me over 800 attempts, and unfortunately it still isn't perfect.
          We don't know the original settings used, so an approximation is needed, and it can still be improved
          over time (it's open source!).
        </p>
      </section>
    </div>
  )
}

function SiteHeader() {
  return (
    <div>
      <h1>PDA Voice Generator</h1>
      <hr />
    </div>
  );
}

function GenerateMainSection() {
  const [modalErrorMessage, setErrorMessage] = React.useState("An error occurred");
  const [modalIsOpen, setIsOpen] = React.useState(false);

  const [downloadUrl, setDownloadUrl] = useState(null);
  const [isPolling, setIsPolling] = useState(false);

  const [useSsml, setUseSsml] = useState(false);

  function openModal() {
    setIsOpen(true);
  }

  function closeModal() {
    setIsOpen(false);
  }

  const [textInput, setText] = useState("Detecting multiple leviathan class lifeforms in the region. Are you certain whatever you're doing is worth it?");

  const handleButtonClick = (setMessage) => {
  (async function () {
    if (textInput.trim() === "") {
      setMessage("Please enter text!");
      openModal();
      return;
    }
    try {
      setDownloadUrl(null);  // clear previous download

      const input = {
        message: textInput,
        use_ssml: useSsml,
        voice_id: "pda"
      };

      const request = { "input": input };

      console.log(JSON.stringify(request));

      const response = await fetch("/api/generate", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(request)
      });

      const json = await response.json();
      if (json.status === "invalid_input") {
        setMessage(json.message);
        openModal();
        return;
      }

      setIsPolling(true);
      const jobId = json.job_id;

      // Polling loop
      const pollInterval = 3000;
      let attempts = 0;
      const maxAttempts = 100; // 20 = 1 minute

      const pollStatus = async () => {
        const statusResponse = await fetch(`/api/status/${jobId}`);
        const statusJson = await statusResponse.json();

        if (statusJson.status === "ready") {
          setDownloadUrl(statusJson.url);
          setIsPolling(false);
        } else if (statusJson.status === "error") {
          setMessage("An internal server error occurred.");
          setIsPolling(false);
          openModal();
        }
        else if (attempts < maxAttempts) {
          attempts++;
          setTimeout(pollStatus, pollInterval);
        } else {
          setMessage("File not ready after waiting.");
          setIsPolling(false);
          openModal();
        }
      };

      pollStatus();

    } catch (error) {
      setMessage("An unknown error occurred.");
      openModal();
    }
  })();
  };

  return (
    <section>
      <h3>The voice filter has been reworked recently to sound more accurate! However, files now take longer to generate.</h3>
      <p>Insert text for voice line generation:</p>
      <textarea
        value={textInput}
        onChange={(e) => setText(e.target.value)}
        style={{
          width: '80%',
          height: '150px',         // default height
          resize: 'vertical',      // allow user to resize up/down only
          padding: '8px',
          fontSize: '1rem',
        }}
      />
      <div class='button-spacing'></div>
      <input
          type="checkbox"
          id="enable-ssml"
          checked={useSsml}
          onChange={e => setUseSsml(e.target.checked)}
      />
      <label htmlFor="enable-ssml">Use SSML (Advanced)</label>
      <div class='button-spacing'></div>
      <button
        type="button"
        onClick={() => {
          handleButtonClick(setErrorMessage);
        }}
        id='generate-button'>
        Generate
      </button>
      <ReactModal
        isOpen={modalIsOpen}
        onRequestClose={closeModal}
        contentLabel="Error while contacting API"
        className="Modal"
        overlayClassName="Overlay"
      >
        <div style={{
          gap: '10px'
        }}>
          <h2>{modalErrorMessage}</h2>
          <button onClick={closeModal}>Close</button>
        </div>
      </ReactModal>
      {isPolling && <p>Your voice line is being generated...</p>}
        {downloadUrl && (
          <div style={{
            marginLeft: '0px'
          }}>
            <h2>Your file is ready!</h2>
            <audio controls>
            <source src={downloadUrl} type="audio/wav" />
            Your browser does not support the audio element.
          </audio>
          <br/>
          <a href={downloadUrl} download target="_blank" rel="noopener noreferrer">
            Download Voice Line
          </a>
          </div>
        )}
    </section>
  )
}

function SiteFooter() {
  return (
    <footer>
      <p>This site is not officially affiliated with Krafton or Unknown Worlds.</p>
      <p>Uploaded text and generated files are stored temporarily. Generated files are deleted within seven days. The tool does NOT use artificial intelligence to generate output files.</p>
    </footer>
  )
}

export default App;
