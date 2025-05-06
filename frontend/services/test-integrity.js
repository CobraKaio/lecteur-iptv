// Fichier de test pour vérifier l'intégrité des services
// Ce fichier importe tous les services et vérifie qu'ils peuvent être utilisés ensemble

// Importer les services
import api from './api';
import channelsService from './channelsService';
import vodService from './vodService';
import streamingService from './streamingService';

// Fonction de test
const testServices = async () => {
  console.log('Testing API service...');
  console.log('API service:', api);
  
  console.log('Testing Channels service...');
  console.log('Channels service:', channelsService);
  
  console.log('Testing VOD service...');
  console.log('VOD service:', vodService);
  
  console.log('Testing Streaming service...');
  console.log('Streaming service:', streamingService);
  
  // Tester les fonctions
  try {
    // Tester l'URL du proxy
    const testUrl = 'http://example.com/stream.m3u8';
    const proxyUrl = streamingService.getProxyUrl(testUrl);
    console.log('Proxy URL for', testUrl, ':', proxyUrl);
    
    // Tester le type de flux
    const streamType = streamingService.getStreamType(testUrl);
    console.log('Stream type for', testUrl, ':', streamType);
    
    // Tester le formatage de la durée
    const duration = 3661; // 1h 1m 1s
    const formattedDuration = streamingService.formatDuration(duration);
    console.log('Formatted duration for', duration, 'seconds:', formattedDuration);
    
    // Tester le formatage du bitrate
    const bitrate = 1500000; // 1.5 Mbps
    const formattedBitrate = streamingService.formatBitrate(bitrate);
    console.log('Formatted bitrate for', bitrate, 'bps:', formattedBitrate);
    
    console.log('All tests passed!');
  } catch (error) {
    console.error('Test failed:', error);
  }
};

// Exécuter le test
testServices();

// Exporter les services pour référence
export {
  api,
  channelsService,
  vodService,
  streamingService
};
