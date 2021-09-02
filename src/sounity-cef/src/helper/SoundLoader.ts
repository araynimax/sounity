export default class SoundLoader {
  private static instance: SoundLoader = null;

  static getInstance(): SoundLoader {
    if (this.instance === null) this.instance = new SoundLoader();

    return this.instance;
  }

  private cache: Record<string, Blob> = {};

  public loadUrl(url: string): Promise<ArrayBuffer> {
    return new Promise(async (resolve, reject) => {
      if (url in this.cache) return resolve(await this.cache[url].arrayBuffer());

      fetch(url)
        .then(async (response) => {
          const blob = await response.blob();
          this.cache[url] = blob;

          return await blob.arrayBuffer();
        })
        .then((buffer) => {
          resolve(buffer);
        })
        .catch(reject);
    });
  }
}
